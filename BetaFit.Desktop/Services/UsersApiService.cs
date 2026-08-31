// =============================================================================
// SenacGames.Desktop - Services/UsuariosApiService.cs
// =============================================================================
//  CONCEITO: Service de Usuários
//
// Gerencia usuários do Identity via API.
//
// IMPORTANTE: A API atual não possui endpoints de gerenciamento de usuários
// (/api/users). Este service está preparado para quando esses endpoints
// forem adicionados à API (UsersController).
//
// Endpoints esperados (a implementar na API se necessário):
//   GET    /api/users             Listar usuários
//   GET    /api/users/{id}        Buscar usuário
//   POST   /api/users             Criar usuário
//   DELETE /api/users/{id}        Excluir usuário
//   POST   /api/users/{id}/roles  Atribuir role
//   POST   /api/users/{id}/reset-password  Redefinir senha
//
// Enquanto não existem, o módulo de usuários usa os endpoints de auth disponíveis.
// =============================================================================


using BetaFit.Desktop.DTOs;
using BetaFit.Desktop.Helpers;


namespace BetaFit.Desktop.Services
{
    /// <summary>
    /// Serviço de comunicação com os endpoints de Usuários da API.
    /// Requer perfil Admin para todas as operações.
    /// </summary>
    public class UsersApiService
    {
        private readonly HttpClientHelper _http;

        public UsersApiService()
        {
            _http = HttpClientHelper.Instance;
        }

        /// <summary>
        /// Lista todos os usuários via GET /api/users.
        /// Endpoint a ser implementado na API (UsersController).
        /// </summary>
        public async Task<List<UsersResponseDto>> GetAllAsync()
        {
            try
            {
                var usuarios = await _http.GetAsync<List<UsersResponseDto>>("/api/usuarios");
                return usuarios ?? new List<UsersResponseDto>();
            }
            catch
            {
                // Retorna lista vazia se o endpoint ainda não existir
                return new List<UsersResponseDto>();
            }
        }

        /// <summary>
        /// Cria um novo usuário via POST /api/users.
        /// </summary>
        public async Task<(bool Success, UsersResponseDto? Usuario, string ErrorMessage)>
            CreateAsync(CreateUsersDto dto)
        {
            return await _http.PostAsync<UsersResponseDto>("/api/usuarios", dto);
        }

        /// <summary>
        /// Exclui um usuário via DELETE /api/users/{id}.
        /// </summary>
        public async Task<(bool Success, string ErrorMessage)> DeleteAsync(string userId)
        {
            return await _http.DeleteAsync($"/api/usuarios/{userId}");
        }

        /// <summary>
        /// Redefine a senha de um usuário.
        /// </summary>
        public async Task<(bool Success, string ErrorMessage)> ResetPasswordAsync(
            ResetPasswordDto dto)
        {
            var (success, _, error) = await _http.PostAsync<object>(
                $"/api/usuarios/{dto.UserId}/reset-password", dto);
            return (success, error);
        }

        /// <summary>
        /// Atribui um perfil (role) a um usuário.
        /// </summary>
        public async Task<(bool Success, string ErrorMessage)> AssignRoleAsync(AssignRoleDto dto)
        {
            var (success, _, error) = await _http.PostAsync<object>(
                $"/api/usuarios/{dto.UserId}/roles", dto);
            return (success, error);
        }

        public async Task<(bool Success, UsersResponseDto? Usuario, string ErrorMessage)>
                UpdateAsync(string id, UpdateUsersDto dto)
        {
            return await _http.PutAsync<UsersResponseDto>($"/api/usuarios/{id}", dto);
        }
    }
}

