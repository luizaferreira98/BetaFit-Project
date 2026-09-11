using BetaFit.API.Services;
using BetaFit.Application.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BetaFit.API.Controllers;

[ApiController]
[Route("api/password")]
public class PasswordController : ControllerBase
{
    private readonly UserManager<IdentityUser> _users;
    private readonly IEmailSender _email;
    private readonly IConfiguration _config;
    public PasswordController(UserManager<IdentityUser> users, IEmailSender email, IConfiguration config)
    { _users = users; _email = email; _config = config; }

    [HttpPost("forgot")]
    public async Task<IActionResult> Forgot(ForgotPasswordDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var user = await _users.FindByEmailAsync(dto.Email.Trim());
        // Always return the same response to avoid account enumeration.
        if (user is null) return Ok(new { message = "Se o e-mail estiver cadastrado, você receberá um link para redefinir sua senha." });
        var token = await _users.GeneratePasswordResetTokenAsync(user);
        var baseUrl = _config["App:PublicBaseUrl"]?.TrimEnd('/') ?? "https://localhost:7000";
        var link = $"{baseUrl}/Account/ResetPassword?email={Uri.EscapeDataString(user.Email!)}&token={Uri.EscapeDataString(token)}";
        var html = $"<h2>Beta Fit</h2><p>Recebemos uma solicitação para redefinir sua senha.</p><p><a href=\"{link}\">Redefinir minha senha</a></p><p>O link expira conforme a configuração de tokens da aplicação.</p>";
        try { await _email.SendAsync(user.Email!, "Redefinição de senha — Beta Fit", html); }
        catch (InvalidOperationException) { return StatusCode(503, new { message = "Não foi possível enviar o e-mail agora. Tente novamente em alguns minutos." }); }
        return Ok(new { message = "Se o e-mail estiver cadastrado, você receberá um link para redefinir sua senha." });
    }

    [HttpPost("reset")]
    public async Task<IActionResult> Reset(ResetPasswordDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (dto.NewPassword != dto.ConfirmPassword) return BadRequest(new { message = "As senhas não coincidem." });
        var user = await _users.FindByEmailAsync(dto.Email.Trim());
        if (user is null) return BadRequest(new { message = "Link de recuperação inválido ou expirado." });
        var result = await _users.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
        if (!result.Succeeded) return BadRequest(new { message = string.Join(" ", result.Errors.Select(x => x.Description)) });
        return Ok(new { message = "Senha redefinida com sucesso." });
    }
}
