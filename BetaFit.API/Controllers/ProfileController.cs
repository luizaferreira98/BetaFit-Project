using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BetaFit.API.Services;
using BetaFit.Application.DTOs;
using BetaFit.Domain.Entities;
using BetaFit.Infraestructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaFit.API.Controllers;

[ApiController, Authorize, Route("api/profile")]
public class ProfileController : ControllerBase
{
    private readonly UserManager<IdentityUser> _users;
    private readonly BetaFitDbContext _db;
    private readonly IEmailSender _email;
    private readonly IConfiguration _config;
    public ProfileController(UserManager<IdentityUser> users, BetaFitDbContext db, IEmailSender email, IConfiguration config)
    { _users=users; _db=db; _email=email; _config=config; }

    [HttpGet]
    public async Task<ActionResult<UserDto>> Get()
    { var user=await _users.GetUserAsync(User); return user is null ? Unauthorized() : Ok(await Map(user)); }

    [HttpGet("pending-change")]
    public async Task<ActionResult<PendingChangeStatusDto>> PendingChange()
    {
        var user = await _users.GetUserAsync(User); if (user is null) return Unauthorized();
        var pending = await _db.PendingProfileChanges.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == user.Id);
        return Ok(new PendingChangeStatusDto {
            Email = pending?.Email ?? "", ExpiresAt = pending?.ExpiresAt,
            IsEmailChange = pending != null && !string.Equals(user.Email, pending.Email, StringComparison.OrdinalIgnoreCase),
            CanConfirm = pending != null && pending.ExpiresAt > DateTime.UtcNow && pending.Attempts < 5
        });
    }

    [HttpPost("email-change")]
    public async Task<ActionResult<ProfileChangeResponseDto>> RequestEmailChange(RequestEmailChangeDto dto)
    {
        var user=await _users.GetUserAsync(User);if(user is null)return Unauthorized();
        if (!string.Equals(dto.CurrentEmail.Trim(), user.Email, StringComparison.OrdinalIgnoreCase)) return BadRequest(new { message="O e-mail atual informado não corresponde à sua conta." });
        if(string.Equals(dto.Email.Trim(),user.Email,StringComparison.OrdinalIgnoreCase))return BadRequest(new{message="Informe um e-mail diferente do atual."});
        var profile=await Map(user);
        // Sensitive changes depend on the existing server profile, not unrelated client fields.
        return await Update(new UpdateProfileDto{Email=dto.Email,CurrentPassword=dto.CurrentPassword,FullName=profile.FullName,PhoneNumber=profile.PhoneNumber,BirthDate=profile.BirthDate??DateTime.Today.AddYears(-18),Cpf=profile.Cpf,Cep=profile.Cep,Street=profile.Street,Number=profile.Number,Complement=profile.Complement,Neighborhood=profile.Neighborhood,City=profile.City,State=profile.State});
    }

    [HttpPut]
    public async Task<ActionResult<ProfileChangeResponseDto>> Update(UpdateProfileDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (dto.BirthDate.Date > DateTime.Today.AddYears(-18) || dto.BirthDate.Date < DateTime.Today.AddYears(-120))
            return BadRequest(new { message = "Informe uma data de nascimento válida e tenha pelo menos 18 anos." });
        var user=await _users.GetUserAsync(User); if(user is null)return Unauthorized();
        var emailChanged=!string.Equals(user.Email,dto.Email.Trim(),StringComparison.OrdinalIgnoreCase);
        if (emailChanged && User.IsInRole("Usuario") && IsInternalEmail(dto.Email)) return BadRequest(new { message="O domínio @betafit é reservado para contas internas." });
        var passwordChanged=!string.IsNullOrWhiteSpace(dto.NewPassword);
        if (passwordChanged && dto.NewPassword != dto.ConfirmNewPassword) return BadRequest(new { message="A nova senha e a confirmação não coincidem." });
        if ((emailChanged || passwordChanged) && string.IsNullOrWhiteSpace(dto.CurrentPassword)) return BadRequest(new { message="Informe a senha atual para confirmar a alteração." });
        if (emailChanged || passwordChanged)
        {
            var check=await _users.CheckPasswordAsync(user,dto.CurrentPassword!);
            if(!check) return BadRequest(new { message="A senha atual está incorreta." });
            var existing=await _db.PendingProfileChanges.Where(x=>x.UserId==user.Id).ToListAsync();
            if(existing.Any(x=>x.RequestedAt>DateTime.UtcNow.AddMinutes(-1)))return StatusCode(429,new{message="Aguarde um minuto antes de solicitar outro código."});
            if(emailChanged){var occupied=await _users.FindByEmailAsync(dto.Email.Trim());if(occupied!=null && occupied.Id!=user.Id)return BadRequest(new{message="Este e-mail já está em uso."});}
            _db.PendingProfileChanges.RemoveRange(existing);
            var raw=RandomNumberGenerator.GetInt32(0,1000000).ToString("D6");
            var hash=Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(user.Id+":"+raw)));
            var pending=new PendingProfileChange{UserId=user.Id,TokenHash=hash,FullName=dto.FullName.Trim(),Email=dto.Email.Trim(),PhoneNumber=dto.PhoneNumber.Trim(), Cpf=dto.Cpf, Cep=dto.Cep, Street=dto.Street, Number=dto.Number, Complement=dto.Complement, Neighborhood=dto.Neighborhood, City=dto.City, State=dto.State, BirthDate=dto.BirthDate,NewPasswordHash=passwordChanged?_users.PasswordHasher.HashPassword(user,dto.NewPassword!):null,ExpiresAt=DateTime.UtcNow.AddMinutes(10)};
            _db.PendingProfileChanges.Add(pending); await _db.SaveChangesAsync();
            var target=emailChanged?dto.Email.Trim():user.Email!;
            var html=$"<h2>Beta Fit</h2><p>Seu código de confirmação é:</p><p style='font-size:32px;font-weight:bold'>{raw}</p><p>Válido por 10 minutos. Se não solicitou, ignore esta mensagem.</p>";
            var baseUrl = _config["App:PublicBaseUrl"]?.TrimEnd('/');
            if (Uri.TryCreate(baseUrl, UriKind.Absolute, out var publicUri) && (publicUri.Scheme == "https" || publicUri.Scheme == "http"))
                html += $"<p><a href='{System.Net.WebUtility.HtmlEncode(baseUrl + "/Account/ChangeEmail")}'>Voltar à página de confirmação</a></p>";
            try { await _email.SendAsync(target,"Confirme uma alteração de segurança — Beta Fit",html); }
            catch(InvalidOperationException ex){_db.PendingProfileChanges.Remove(pending);await _db.SaveChangesAsync();return StatusCode(503,new{message=ex.Message});}
            return Ok(new ProfileChangeResponseDto{RequiresVerification=true,Message=_config["Email:Mode"]=="Outbox"?"Modo de teste: nenhuma mensagem foi enviada. Consulte o código no terminal da API. Seu e-mail ainda não foi alterado.":"Código enviado ao novo e-mail. Confira a caixa de entrada e o spam. Seu e-mail atual permanece até a confirmação."});
        }
        await ApplyAsync(user,dto,null); return Ok(new ProfileChangeResponseDto{User=await Map(user),Message="Perfil atualizado com sucesso."});
    }

    [HttpPut("checkout-address")]
    public async Task<IActionResult> SaveCheckoutAddress(CheckoutAddressDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var user = await _users.GetUserAsync(User);
        if (user is null) return Unauthorized();

        var claims = await _users.GetClaimsAsync(user);
        var types = new[] { "Cpf", "Address.Cep", "Address.Street", "Address.Number", "Address.Complement", "Address.Neighborhood", "Address.City", "Address.State" };
        var old = claims.Where(c => types.Contains(c.Type)).ToList();
        if (old.Count > 0) await _users.RemoveClaimsAsync(user, old);
        await _users.AddClaimsAsync(user, new[]
        {
            new Claim("Cpf", new string(dto.Cpf.Where(char.IsDigit).ToArray())),
            new Claim("Address.Cep", dto.Cep.Trim()), new Claim("Address.Street", dto.Street.Trim()),
            new Claim("Address.Number", dto.Number.Trim()), new Claim("Address.Neighborhood", dto.Neighborhood.Trim()),
            new Claim("Address.City", dto.City.Trim()), new Claim("Address.State", dto.State.Trim().ToUpperInvariant())
        });
        if (!string.IsNullOrWhiteSpace(dto.Complement))
            await _users.AddClaimAsync(user, new Claim("Address.Complement", dto.Complement.Trim()));
        return NoContent();
    }

    [HttpPut("card")]
    public async Task<IActionResult> SaveCard(PaymentCardDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var user = await _users.GetUserAsync(User); if (user is null) return Unauthorized();
        var number = new string(dto.CardNumber.Where(char.IsDigit).ToArray());
        if (!PassesLuhn(number)) return BadRequest(new { message = "Número de cartão inválido. Confira os dígitos ou use a opção Preencher cartão de teste (4111 1111 1111 1111)." });
        if (!IsFutureExpiry(dto.Expiry)) return BadRequest(new { message = "A validade do cartão deve estar no futuro." });
        var claims = await _users.GetClaimsAsync(user);
        if(BetaFit.Application.Services.DemoWallet.Read(claims).Count>=20) return BadRequest(new{message="Limite de 20 cartões. Remova um cartão para adicionar outro."});
        var card=new SavedCardDto{Holder=dto.CardHolderName.Trim(),Brand=DetectBrand(number),Last4=number[^4..],Expiry=dto.Expiry,Type=dto.Type};
        var result=await _users.AddClaimAsync(user,new Claim("DemoCard",System.Text.Json.JsonSerializer.Serialize(card)));
        return result.Succeeded ? Ok(card) : StatusCode(500,new{message="Não foi possível salvar o cartão."});
    }

    [HttpDelete("cards/{id}")]
    public async Task<IActionResult> DeleteCard(string id)
    {
        var user=await _users.GetUserAsync(User); if(user is null)return Unauthorized();
        var claims=await _users.GetClaimsAsync(user);
        var found=claims.Where(c=>id=="legacy" ? c.Type.StartsWith("Card.") : c.Type=="DemoCard" && System.Text.Json.JsonSerializer.Deserialize<SavedCardDto>(c.Value)?.Id==id).ToList();
        if(found.Count==0)return NotFound();
        var result=await _users.RemoveClaimsAsync(user,found); return result.Succeeded ? NoContent() : StatusCode(500);
    }

    [HttpPost("confirm-change")]
    public async Task<ActionResult<ProfileChangeResponseDto>> Confirm([FromBody] ConfirmProfileChangeDto dto)
    {
        if(!ModelState.IsValid)return ValidationProblem(ModelState);
        var user=await _users.GetUserAsync(User); if(user is null)return Unauthorized();
        await using var transaction=await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var pending=await _db.PendingProfileChanges.FirstOrDefaultAsync(x=>x.UserId==user.Id);
        if(pending is null || pending.ExpiresAt<=DateTime.UtcNow || pending.Attempts>=5)return BadRequest(new{message="Código expirado ou bloqueado. Solicite outro."});
        var hash=Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(user.Id+":"+dto.Token)));
        if(!CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(pending.TokenHash),Encoding.UTF8.GetBytes(hash)))
        {
            pending.Attempts++;await _db.SaveChangesAsync();await transaction.CommitAsync();return BadRequest(new{message="Código incorreto. Após 5 tentativas, solicite outro."});
        }
        if(!string.Equals(user.Email,pending.Email,StringComparison.OrdinalIgnoreCase)){
            var email=await _users.FindByEmailAsync(pending.Email); if(email is not null && email.Id!=user.Id)return BadRequest(new{message="Este e-mail já está em uso."});
            user.EmailConfirmed=true; user.Email=pending.Email; user.UserName=pending.Email; user.NormalizedEmail=_users.NormalizeEmail(pending.Email); user.NormalizedUserName=_users.NormalizeName(pending.Email);
        }
        user.PhoneNumber=pending.PhoneNumber; user.PasswordHash=pending.NewPasswordHash ?? user.PasswordHash;
        var update=await _users.UpdateAsync(user); if(!update.Succeeded)return BadRequest(new{message=string.Join(" ",update.Errors.Select(e=>e.Description))});
        var claims=await _users.GetClaimsAsync(user); var types=new[]{"FullName","BirthDate","Cpf","Address.Cep","Address.Street","Address.Number","Address.Complement","Address.Neighborhood","Address.City","Address.State"}; var old=claims.Where(c=>types.Contains(c.Type)).ToList(); if(old.Count>0)await _users.RemoveClaimsAsync(user,old); var newClaims=new List<Claim>{new("FullName",pending.FullName),new("BirthDate",pending.BirthDate.ToString("yyyy-MM-dd"))}; if(!string.IsNullOrWhiteSpace(pending.Cpf))newClaims.Add(new Claim("Cpf",pending.Cpf)); if(!string.IsNullOrWhiteSpace(pending.Cep))newClaims.Add(new Claim("Address.Cep",pending.Cep)); if(!string.IsNullOrWhiteSpace(pending.Street))newClaims.Add(new Claim("Address.Street",pending.Street)); if(!string.IsNullOrWhiteSpace(pending.Number))newClaims.Add(new Claim("Address.Number",pending.Number)); if(!string.IsNullOrWhiteSpace(pending.Complement))newClaims.Add(new Claim("Address.Complement",pending.Complement)); if(!string.IsNullOrWhiteSpace(pending.Neighborhood))newClaims.Add(new Claim("Address.Neighborhood",pending.Neighborhood)); if(!string.IsNullOrWhiteSpace(pending.City))newClaims.Add(new Claim("Address.City",pending.City)); if(!string.IsNullOrWhiteSpace(pending.State))newClaims.Add(new Claim("Address.State",pending.State)); await _users.AddClaimsAsync(user,newClaims);
        await _db.PendingProfileChanges.Where(x=>x.UserId==user.Id).ExecuteDeleteAsync();
        await _users.UpdateSecurityStampAsync(user);
        await transaction.CommitAsync();
        return Ok(new ProfileChangeResponseDto{User=await Map(user),Message="Alterações confirmadas com sucesso."});
    }

    private async Task ApplyAsync(IdentityUser user,UpdateProfileDto dto,string? passwordHash)
    {
        user.PhoneNumber=dto.PhoneNumber.Trim(); if(passwordHash!=null)user.PasswordHash=passwordHash; var result=await _users.UpdateAsync(user); if(!result.Succeeded)throw new InvalidOperationException(string.Join(" ",result.Errors.Select(e=>e.Description)));
        var claims=await _users.GetClaimsAsync(user); var types=new[]{"FullName","BirthDate","Cpf","Address.Cep","Address.Street","Address.Number","Address.Complement","Address.Neighborhood","Address.City","Address.State"}; var old=claims.Where(c=>types.Contains(c.Type)).ToList(); if(old.Count>0)await _users.RemoveClaimsAsync(user,old);
        var newClaims=new List<Claim>{new("FullName",dto.FullName.Trim()),new("BirthDate",dto.BirthDate.ToString("yyyy-MM-dd"))}; if(!string.IsNullOrWhiteSpace(dto.Cpf))newClaims.Add(new Claim("Cpf",new string(dto.Cpf.Where(char.IsDigit).ToArray()))); if(!string.IsNullOrWhiteSpace(dto.Cep))newClaims.Add(new Claim("Address.Cep",dto.Cep)); if(!string.IsNullOrWhiteSpace(dto.Street))newClaims.Add(new Claim("Address.Street",dto.Street.Trim())); if(!string.IsNullOrWhiteSpace(dto.Number))newClaims.Add(new Claim("Address.Number",dto.Number.Trim())); if(!string.IsNullOrWhiteSpace(dto.Complement))newClaims.Add(new Claim("Address.Complement",dto.Complement.Trim())); if(!string.IsNullOrWhiteSpace(dto.Neighborhood))newClaims.Add(new Claim("Address.Neighborhood",dto.Neighborhood.Trim())); if(!string.IsNullOrWhiteSpace(dto.City))newClaims.Add(new Claim("Address.City",dto.City.Trim())); if(!string.IsNullOrWhiteSpace(dto.State))newClaims.Add(new Claim("Address.State",dto.State.Trim().ToUpperInvariant())); await _users.AddClaimsAsync(user,newClaims);
    }
    private async Task<UserDto> Map(IdentityUser user){var claims=await _users.GetClaimsAsync(user);DateTime? birth=DateTime.TryParse(claims.FirstOrDefault(c=>c.Type=="BirthDate")?.Value,out var b)?b:null;return new UserDto{Id=user.Id,Email=user.Email??string.Empty,FullName=claims.FirstOrDefault(c=>c.Type=="FullName")?.Value??user.UserName?.Split('@')[0]??string.Empty,PhoneNumber=user.PhoneNumber??string.Empty,BirthDate=birth,Gender=claims.FirstOrDefault(c=>c.Type=="Gender")?.Value,Cpf=claims.FirstOrDefault(c=>c.Type=="Cpf")?.Value,Cep=claims.FirstOrDefault(c=>c.Type=="Address.Cep")?.Value,Street=claims.FirstOrDefault(c=>c.Type=="Address.Street")?.Value,Number=claims.FirstOrDefault(c=>c.Type=="Address.Number")?.Value,Complement=claims.FirstOrDefault(c=>c.Type=="Address.Complement")?.Value,Neighborhood=claims.FirstOrDefault(c=>c.Type=="Address.Neighborhood")?.Value,City=claims.FirstOrDefault(c=>c.Type=="Address.City")?.Value,State=claims.FirstOrDefault(c=>c.Type=="Address.State")?.Value,CardHolderName=claims.FirstOrDefault(c=>c.Type=="Card.Holder")?.Value,CardBrand=claims.FirstOrDefault(c=>c.Type=="Card.Brand")?.Value,CardLast4=claims.FirstOrDefault(c=>c.Type=="Card.Last4")?.Value,CardExpiry=claims.FirstOrDefault(c=>c.Type=="Card.Expiry")?.Value,Cards=BetaFit.Application.Services.DemoWallet.Read(claims),Roles=await _users.GetRolesAsync(user)};}

    private static bool PassesLuhn(string number){var sum=0;var alternate=false;for(var i=number.Length-1;i>=0;i--){var n=number[i]-'0';if(alternate){n*=2;if(n>9)n-=9;}sum+=n;alternate=!alternate;}return number.Length is >=13 and <=19 && sum%10==0;}
    private static bool IsFutureExpiry(string expiry){var parts=expiry.Split('/');if(parts.Length!=2||!int.TryParse(parts[0],out var month)||!int.TryParse(parts[1],out var year)||month is <1 or >12)return false;var lastDay=new DateTime(2000+year,month,DateTime.DaysInMonth(2000+year,month));return lastDay>=DateTime.Today;}
    private static string DetectBrand(string number)=>number.StartsWith("4")?"Visa":number.StartsWith("5")?"Mastercard":number.StartsWith("34")||number.StartsWith("37")?"American Express":number.StartsWith("6")?"Elo/Discover":"Cartão";
    private static bool IsInternalEmail(string? email){var domain=email?.Trim().Split('@').LastOrDefault();return !string.IsNullOrWhiteSpace(domain)&&(domain.Equals("betafit",StringComparison.OrdinalIgnoreCase)||domain.EndsWith(".betafit",StringComparison.OrdinalIgnoreCase)||domain.Equals("betafit.com",StringComparison.OrdinalIgnoreCase));}
}
