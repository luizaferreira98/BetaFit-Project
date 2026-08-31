using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetaFit.Desktop.DTOs
{
    /// <summary>
    /// DTO para representar os dados de login enviados para a API.
    /// Mapeia o JSON enviado no corpo do POST /api/auth/login
    /// </summary>
    public class LoginRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para registrar um novo usuário.
    /// Mapeia o JSON enviado no POST /api/auth/register
    /// </summary>  
    public class RegisterRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
