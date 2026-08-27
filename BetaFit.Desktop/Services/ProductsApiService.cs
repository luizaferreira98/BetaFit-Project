// =============================================================================
// BetaFit.Desktop - Services/ProductsApiService.cs
// =============================================================================
//  CONCEITO: Service de Produtos
//
// Realiza todas as operações CRUD de produtos via API REST:
//   GET    /api/products         Listar todos os produtos
//   GET    /api/products/{id}    Buscar produto por ID
//   POST   /api/products         Criar produto (requer Admin)
//   PUT    /api/products/{id}    Atualizar produto (requer Admin)
//   DELETE /api/products/{id}    Excluir produto (requer Admin)
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
    public class ProductsApiService
    {
        private readonly HttpClientHelper _http;

        // Construtor - Inicializa junto com o código quando o mesmo é chamado.
        public ProductsApiService()
        {
            _http = HttpClientHelper.Instance;
        }

        /// <summary>
        /// Lista todos os produtos via GET /api/products
        /// </summary>
        public async Task<List<ProductResponseDto>> GetAllAsync()
        {
            try
            {
                var produtos = await _http.GetAsync<List<ProductResponseDto>>("/api/products");
                return produtos ?? new List<ProductResponseDto>();
            }
            catch
            {
                return new List<ProductResponseDto>();
            }
        }

        /// <summary>
        /// Busca um produto específico por ID via GET /api/products/{id}
        /// </summary>
        public async Task<ProductResponseDto?> GetByIdAsync(int id)
        {
            return await _http.GetAsync<ProductResponseDto>($"/api/products/{id}");
        }

        /// <summary>
        /// Cria um novo produto via POST /api/products.
        /// Requer perfil Admin (verificado pela API).
        /// </summary>
        /// <param name="dto">Dados do produto a ser criado</param>
        /// <returns>Produto criado ou null em caso de erro</returns>
        public async Task<(bool Success, ProductResponseDto? Product, string ErrorMessage)>
            CreateAsync(CreateProductDto dto)
        {
            return await _http.PostAsync<ProductResponseDto>("/api/products", dto);
        }

        /// <summary>
        /// Atualiza um produto existente via PUT /api/products/{id}.
        /// Requer perfil Admin (verificado pela API).
        /// </summary>
        public async Task<(bool Success, ProductResponseDto? Product, string ErrorMessage)>
            UpdateAsync(int id, UpdateProductDto dto)
        {
            return await _http.PutAsync<ProductResponseDto>($"/api/products/{id}", dto);
        }

        /// <summary>
        /// Exclui um produto via DELETE /api/products/{id}.
        /// Requer perfil Admin (verificado pela API).
        /// </summary>
        public async Task<(bool Success, string ErrorMessage)> DeleteAsync(int id)
        {
            return await _http.DeleteAsync($"/api/products/{id}");
        }
    }
}
