// =============================================================================
// BetaFit.Desktop - Services/GamesApiService.cs
// =============================================================================
//  CONCEITO: Service de Games
//
// Realiza todas as operações CRUD de games via API REST:
//   GET    /api/Products         Listar todos os games
//   GET    /api/Products/{id}    Buscar game por ID
//   POST   /api/Products         Criar game (requer Admin)
//   PUT    /api/Products/{id}    Atualizar game (requer Admin)
//   DELETE /api/Products/{id}    Excluir game (requer Admin)
//
// IMPORTANTE: As operações de escrita (POST, PUT, DELETE) requerem
// que o usuário esteja autenticado como Admin.
// A autorização é verificada pela própria API, não pelo Desktop.
// O Desktop não precisa verificar roles para fazer a chamada —
// mas deve controlar a INTERFACE (exibir/ocultar botões) baseado no perfil.
// =============================================================================

using BetaFit.Desktop.Helpers;
using BetaFit.Desktop.DTOs;


namespace BetaFit.Desktop.Services
{
    public class GamesApiService
    {
        private readonly HttpClientHelper _http;

        //Construtor - Inicializa junto com o código quando o mesmo é chamado.
        public GamesApiService()
        {
            _http = HttpClientHelper.Instance;
        }

        ///<summary>
        /// Lista todas os ganes via GET /api/Products
        /// </summary>
        public async Task<List<ProductResponseDto>> GetAllAsync()
        {
            try
            {
                var games = await _http.GetAsync<List<ProductResponseDto>>("/api/products");
                return games ?? new List<ProductResponseDto>();
            }
            catch
            {
                return new List<ProductResponseDto>();
            }
        }

        /// <summary>
        /// Busca um game específico por ID via GET /api/Products/{id} 
        /// </summary>
        public async Task<ProductResponseDto> GetByIdAsync(int id)
        {
            return await _http.GetAsync<ProductResponseDto>($"/api/products/{id}");
        }

        /// <summary>
        /// Cria um novo game via POST /api/Products.
        /// Requer perfil Admin (verificado pela API).
        /// </summary>
        /// <param name="dto">Dados do game a ser criado</param>
        /// <returns>Game criado ou null em caso de erro</returns>
        public async Task<(bool Success, ProductResponseDto? Product, string ErrorMessage)>
            CreateAsync(CreateProductDto dto)
        {
            return await _http.PostAsync<ProductResponseDto>("/api/products", dto);
        }

        /// <summary>
        /// Atualiza um game existente via PUT /api/Products/{id}.
        /// Requer perfil Admin (verificado pela API).
        /// </summary>
        public async Task<(bool Success, ProductResponseDto? Product, string ErrorMessage)>
            UpdateAsync(int id, UpdateProductDto dto)
        {
            return await _http.PutAsync<ProductResponseDto>($"/api/products/{id}", dto);
        }

        /// <summary>
        /// Exclui um game via DELETE /api/Products/{id}.
        /// Requer perfil Admin (verificado pela API).
        /// </summary>
        public async Task<(bool Success, string ErrorMessage)> DeleteAsync(int id)
        {
            return await _http.DeleteAsync($"/api/products/{id}");
        }
    }



}
