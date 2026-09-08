using System.ComponentModel.DataAnnotations;

namespace BetaFit.Application.DTOs;

public class UsuarioDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}

public class CreateUsuarioDto
{
    [Required, EmailAddress, StringLength(256)] public string Email { get; set; } = string.Empty;
    [Required, StringLength(120)] public string UserName { get; set; } = string.Empty;
    [Required, StringLength(100, MinimumLength = 6)] public string Password { get; set; } = string.Empty;
    [Required, StringLength(100)] public string ConfirmPassword { get; set; } = string.Empty;
    [Required, StringLength(30)] public string Role { get; set; } = "Usuario";
}

public class UpdateUsuarioDto
{
    [Required, EmailAddress, StringLength(256)] public string Email { get; set; } = string.Empty;
    [StringLength(100, MinimumLength = 6)] public string? Password { get; set; }
    [StringLength(100)] public string? ConfirmPassword { get; set; }
    [Required, StringLength(30)] public string Role { get; set; } = string.Empty;
}

public sealed class AssignRoleDto
{
    [Required, StringLength(30)] public string Role { get; set; } = "Usuario";
}
