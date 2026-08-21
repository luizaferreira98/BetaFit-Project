// =============================================================================
// BetaFit.Desktop - Services/CategoriasApiService.cs
// =============================================================================
//  CONCEITO: Service de Categorias
//
// Realiza operações CRUD de categorias via API REST:
//   GET    /api/categories         Listar todas as categorias
//   POST   /api/categories         Criar categoria (requer Admin)
//   PUT    /api/categories/{id}    Atualizar categoria (requer Admin)
//   DELETE /api/categories/{id}    Excluir categoria (requer Admin)
//
// Observe que NÃO existe GET /api/categories/{id} — a API atual
// não implementa busca por ID, apenas listagem completa.
// =============================================================================

using BetaFit.Desktop.DTOs;
using BetaFit.Desktop.Helpers;


namespace BetaFit.Desktop.Services
{
    public class CategoriesApiService
    {
        private readonly HttpClientHelper _http;

        public CategoriesApiService()
        {
            _http = HttpClientHelper.Instance;
        }

        ///<summary>
        /// Lista todas as categorias via GET /api/categories
        /// </summary>
        public async Task<List<CategoriaResponseDto>> GetAllAsync()
        {
            try
            {
                var categorias = await _http.GetAsync<List<CategoriaResponseDto>>("/api/categories");
                return categorias ?? new List<CategoriaResponseDto>();
            }
            catch
            {
                return new List<CategoriaResponseDto>();
            }
        }

        /// <summary>
        /// Cria uma categoria via POST /api/categories
        /// Requer perfil Admin
        /// </summary>
        public async Task<(bool Success, CategoriaResponseDto? Categoria, string ErrorMessage)>
            CreateAsync(CreateCategoriaDto dto)
        {
            return await _http.PostAsync<CategoriaResponseDto>("/api/categories", dto);
        }

        ///<summary>
        ///Atualiza uma categoria via PUT /api/categories/{id}
        ///Requer pefil Admin
        /// </summary>
        public async Task<(bool Success, CategoriaResponseDto? Categoria, string ErrorMessage)>
           UpdateAsync(int id, UpdateCategoriaDto dto)
        {
            return await _http.PutAsync<CategoriaResponseDto>($"/api/categories/{id}", dto);
        }

        ///<summary>
        ///Exclui uma categoria via DELETE /api/categories/{id}
        ///Requer pefil Admin
        /// </summary>
        public async Task<(bool Success, string ErrorMessage)>
           DeleteAsync(int id)
        {
            return await _http.DeleteAsync($"/api/categories/{id}");
        }




    }
}
