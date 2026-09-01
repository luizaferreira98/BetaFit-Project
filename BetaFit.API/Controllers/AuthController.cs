using System.Security.Claims;
using BetaFit.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BetaFit.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        { _userManager = userManager; _signInManager = signInManager; }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            if (dto.Password != dto.ConfirmPassword) return BadRequest(new { message = "As senhas não coincidem." });
            if (dto.BirthDate.Date > DateTime.Today.AddYears(-18))
                return BadRequest(new { message = "É necessário ter 18 anos ou mais para criar uma conta." });
            if (dto.BirthDate.Date < DateTime.Today.AddYears(-120))
                return BadRequest(new { message = "Informe uma data de nascimento válida." });

            var user = new IdentityUser { UserName = dto.Email.Trim(), Email = dto.Email.Trim(), PhoneNumber = dto.PhoneNumber.Trim() };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return BadRequest(new { message = "Erro ao registrar.", errors = result.Errors.Select(e => e.Description) });

            await _userManager.AddClaimsAsync(user, new[]
            {
                new Claim("FullName", dto.FullName.Trim()),
                new Claim("BirthDate", dto.BirthDate.ToString("yyyy-MM-dd"))
            });
            await _userManager.AddToRoleAsync(user, "Usuario");
            return Ok(new { message = "Usuário registrado com sucesso!" });
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, false, false);
            if (!result.Succeeded) return Unauthorized(new { message = "Login ou senha inválidos." });
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null) return Unauthorized();
            return Ok(await BuildUserDto(user));
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        { await _signInManager.SignOutAsync(); return Ok(new { message = "Logout realizado com sucesso!" }); }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDto>> Me()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized();
            return Ok(await BuildUserDto(user));
        }

        private async Task<UserDto> BuildUserDto(IdentityUser user)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var birth = claims.FirstOrDefault(c => c.Type == "BirthDate")?.Value;
            DateTime? birthDate = DateTime.TryParse(birth, out var parsed) ? parsed : null;
            return new UserDto
            {
                Id = user.Id, Email = user.Email ?? string.Empty, FullName = claims.FirstOrDefault(c => c.Type == "FullName")?.Value ?? user.UserName ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty, BirthDate = birthDate, Roles = await _userManager.GetRolesAsync(user)
            };
        }
    }
}
