using System;
using System.Collections.Generic;

namespace BetaFit.Desktop.DTOs
{
    /// <summary>
    /// DTO de resposta do perfil do usuário logado.
    /// Espelha o UserDto retornado por GET/PUT /api/profile na API.
    /// </summary>
    public class ProfileResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public List<string> Roles { get; set; } = new();

        public bool IsAdmin =>
            Roles.Exists(r => string.Equals(r, "Admin", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// DTO enviado em PUT /api/profile.
    /// FullName, Email, PhoneNumber e BirthDate são obrigatórios na API
    /// (todos marcados [Required] no UpdateProfileDto do lado do servidor) —
    /// mesmo que a troca de senha não esteja sendo usada nesta chamada.
    /// CurrentPassword/NewPassword/ConfirmNewPassword só precisam ser
    /// preenchidos quando o usuário está trocando a senha.
    /// </summary>
    public class UpdateProfileDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmNewPassword { get; set; }
    }
}