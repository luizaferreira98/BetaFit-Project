// =============================================================================
// BetaFit.Desktop - Services/ProfileApiService.cs
// =============================================================================
//  CONCEITO: Service de Perfil (Meu Perfil)
//
// Realiza a leitura e atualização dos dados do usuário logado via API REST:
//   GET /api/profile   Busca os dados do usuário autenticado (via cookie)
//   PUT /api/profile   Atualiza nome/e-mail/telefone/nascimento (e opcionalmente a senha)
//
// Este service não existia — a UserControl de Perfil (PerfilUserControl)
// estava vazia, sem nenhuma chamada à API.
// =============================================================================

using BetaFit.Desktop.DTOs;
using BetaFit.Desktop.Helpers;

namespace BetaFit.Desktop.Services
{
    public class ProfileApiService
    {
        private readonly HttpClientHelper _http;

        public ProfileApiService()
        {
            _http = HttpClientHelper.Instance;
        }

        /// <summary>
        /// Busca os dados do perfil do usuário logado via GET /api/profile.
        /// </summary>
        public async Task<ProfileResponseDto?> GetAsync()
        {
            try
            {
                return await _http.GetAsync<ProfileResponseDto>("/api/profile");
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Atualiza os dados do perfil via PUT /api/profile.
        /// </summary>
        public async Task<(bool Success, ProfileResponseDto? Profile, string ErrorMessage)>
            UpdateAsync(UpdateProfileDto dto)
        {
            return await _http.PutAsync<ProfileResponseDto>("/api/profile", dto);
        }
    }
}