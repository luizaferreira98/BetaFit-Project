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
<<<<<<< HEAD
        public string Login { get; set; } = string.Empty;
=======
        public string Email { get; set; } = string.Empty;
>>>>>>> 9aa9898d3f31f5e28e0f9360943b6a83a794de8c
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

    /// <summary>
    /// DTO que representa o usuário autenticado retornado pela API após login.
    /// Mapeia o JSON retornando no POST /api/auth/login e GET /api/auth/me
    ///</summary>

    public class UserResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();

        /// <summary>
        /// Verifica se o usuário possui a role "Admin" e retorna true ou false.
        /// usando controle de acesso na interface
        /// </summary>
        public bool IsAdmin => Roles.Contains("Admin");
    }
}
