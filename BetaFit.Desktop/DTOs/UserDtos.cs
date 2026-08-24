namespace BetaFit.Desktop.DTOs;

public sealed class UsuarioDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();

    public string Funcao => Roles.Any(r => string.Equals(r, "Admin", StringComparison.OrdinalIgnoreCase))
        ? "ADMIN" : "USUÁRIO STANDARD";

    // A API atual não expõe IsActive. Enquanto isso, usuário retornado pela API é tratado como ativo.
    public bool Ativo { get; set; } = true;
}

public sealed class CreateUsuarioDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string Role { get; set; } = "Usuario";
}

public sealed class UpdateUsuarioDto
{
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    public string Role { get; set; } = "Usuario";
}

public sealed class ResetPasswordDto
{
    public string UserId { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public sealed class AssignRoleDto
{
    public string UserId { get; set; } = string.Empty;
    public string Role { get; set; } = "Usuario";
}
