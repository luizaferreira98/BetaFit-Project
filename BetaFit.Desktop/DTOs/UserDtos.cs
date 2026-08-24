namespace BetaFit.Desktop.DTOs
{
    /// <summary>
    /// DTO para representar um Usuário retornado da API.
    /// </summary>
    public class UsuarioResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        /// <summary> Lista de roles(perfil) do usuário. </summary>
        public List<string> Roles { get; set; } = new();

        /// <summary>
        /// Retorna o perfil principal do usuário como string formatada
        /// Útil para exibição no DataGridView
        /// </summary>

        public string PerfilPrincipal =>
            Roles.Contains("Admin") ? "Administrador" :
            Roles.Count > 0 ? string.Join(", ", Roles) : "Usuário Comum";
    }

    /// <summary>
    /// DTO para a criação de um novo Usuário
    ///</summary>  
    public class CreateUsuarioDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Role { get; set; } = "Usuário";
    }

    /// <summary>
    /// DTO para redefinição de senha de um Usuário
    ///</summary>
    public class ResetPasswordDto
    {
        public string UserId { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para atribuição/remoção de role(perfil de usuário)
    ///</summary>
    public class AssignRoleDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class UpdateUsuarioDto
    {
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string Role { get; set; } = string.Empty;
    }

}
