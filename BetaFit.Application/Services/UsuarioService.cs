using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace BetaFit.Application.Services
{
    public class UsuariosService : IUsuariosService
    {
        // O ASP.NET Injeta (Dependency Injection) essas classes automaticamente pra nós!
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuariosService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<UsuarioDto>> GetAllAsync()
        {
            var users = _userManager.Users.ToList();
            var result = new List<UsuarioDto>();

            // Iteramos sobre os usuários do banco e transformamos em UsuarioDto
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new UsuarioDto
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Roles = roles.ToList()
                });
            }

            return result;
        }

        public async Task<UsuarioDto?> GetByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new UsuarioDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = roles.ToList()
            };
        }

        public async Task<(bool Success, UsuarioDto? Usuario, string ErrorMessage)> CreateAsync(CreateUsuarioDto dto)
        {
            // Validação simples
            if (dto.Password != dto.ConfirmPassword)
                return (false, null, "As senhas não coincidem.");

            // Criar o modelo base do Identity (username = email, já que não há campo "Nome")
            var user = new IdentityUser { UserName = dto.Email, Email = dto.Email };

            // Aqui a mágica do Hash de senha acontece
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var mensagens = string.Join(" ", result.Errors.Select(e => e.Description));
                return (false, null, string.IsNullOrWhiteSpace(mensagens) ? "Erro ao criar usuário." : mensagens);
            }

            // Adiciona o perfil (ex: "Admin" ou "Usuario")
            var role = NormalizeRole(dto.Role);
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);

            var createdUser = new UsuarioDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = new List<string> { role }
            };

            return (true, createdUser, string.Empty);
        }

        public async Task<(bool Success, UsuarioDto? Usuario, string ErrorMessage)> UpdateAsync(string id, UpdateUsuarioDto dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return (false, null, "Usuário não encontrado.");

            user.UserName = dto.Email;
            user.Email = dto.Email;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var mensagens = string.Join(" ", updateResult.Errors.Select(e => e.Description));
                return (false, null, string.IsNullOrWhiteSpace(mensagens) ? "Erro ao atualizar usuário." : mensagens);
            }

            // Se enviou uma nova senha, podemos atualizar também
            if (!string.IsNullOrEmpty(dto.Password))
            {
                if (dto.Password != dto.ConfirmPassword)
                    return (false, null, "As senhas não coincidem.");

                // Remover senha antiga e adicionar a nova (via token de reset)
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passResult = await _userManager.ResetPasswordAsync(user, token, dto.Password);
                if (!passResult.Succeeded)
                {
                    var mensagens = string.Join(" ", passResult.Errors.Select(e => e.Description));
                    return (false, null, string.IsNullOrWhiteSpace(mensagens) ? "Erro ao atualizar a senha." : mensagens);
                }
            }

            var roles = await _userManager.GetRolesAsync(user);
            var requestedRole = NormalizeRole(dto.Role);
            if (!roles.Contains(requestedRole))
            {
                // Garante que a role exista antes de atribuí-la
                if (!await _roleManager.RoleExistsAsync(requestedRole))
                    await _roleManager.CreateAsync(new IdentityRole(requestedRole));

                // Remove as roles antigas e atribui a nova (usuário tem sempre 1 perfil principal)
                if (roles.Count > 0)
                    await _userManager.RemoveFromRolesAsync(user, roles);

                await _userManager.AddToRoleAsync(user, requestedRole);
            }

            var rolesAtualizadas = await _userManager.GetRolesAsync(user);
            var updatedUser = new UsuarioDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = rolesAtualizadas.ToList()
            };

            return (true, updatedUser, string.Empty);
        }

        public async Task<(bool Success, string ErrorMessage)> DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return (false, "Usuário não encontrado.");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded) return (false, "Erro ao excluir o usuário.");

            return (true, string.Empty);
        }

        public async Task<IEnumerable<string>> GetPerfisAsync()
        {
            // Retorna a lista de nomes dos perfis cadastrados no Identity
            await Task.CompletedTask;
            return new List<string> { "Admin", "Funcionario", "Usuario" };
        }

        private static string NormalizeRole(string? role) => role?.Trim().ToLowerInvariant() switch
        {
            "admin" => "Admin",
            "funcionario" or "funcionário" => "Funcionario",
            "usuario" or "usuário" or null or "" => "Usuario",
            _ => throw new InvalidOperationException("Selecione uma função válida: Admin, Funcionario ou Usuario.")
        };
    }
}
