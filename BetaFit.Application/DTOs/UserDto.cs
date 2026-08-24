namespace BetaFit.Application.DTOs
{
    // DTO usado para listar usuários com suas roles
    public class UsuarioDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }

    // DTO usado APENAS quando formos criar um usuário novo
    public class CreateUsuarioDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Role { get; set; } = "Usuário";
    }

    // DTO para atualizar (senha é opcional)
    public class UpdateUsuarioDto
    {
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}