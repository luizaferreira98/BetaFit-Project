// =============================================================================
// BetaFit.UI - Services/HttpProductService.cs
// =============================================================================
//  CONCEITO: Implementação HTTP da IProductService
// A camada Application define O CONTRATO (IProductService). Aqui, na UI,
// implementamos esse contrato consumindo a BetaFit.API via HttpClient —
// exatamente a mesma interface que a API usa internamente com o
// repositório, só que do outro lado da rede. O Controller não sabe (nem
// precisa saber) que a implementação é HTTP.
// =============================================================================

using System.Net.Http.Json;
using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;

namespace BetaFit.UI.Services
{
    public class HttpProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public HttpProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ProductDto>>("api/Products") ?? new List<ProductDto>();
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/Products/{id}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<ProductDto>();
        }

        public async Task<IEnumerable<ProductDto>> GetFeaturedAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ProductDto>>("api/Products/featured") ?? new List<ProductDto>();
        }

        public async Task<IEnumerable<ProductDto>> GetByCategoryAsync(int categoryId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ProductDto>>($"api/Products/category/{categoryId}") ?? new List<ProductDto>();
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Products", dto);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<ProductDto>())!;
        }

        public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Products/{id}", dto);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<ProductDto>();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Products/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<int> CountAsync()
        {
            var products = await GetAllAsync();
            return products.Count();
        }
    }
}
