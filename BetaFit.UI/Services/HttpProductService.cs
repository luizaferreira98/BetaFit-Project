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
using BetaFit.UI.Helpers;

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
            var products = await _httpClient.GetFromJsonAsync<IEnumerable<ProductDto>>("api/Products") ?? Enumerable.Empty<ProductDto>();
            return products.Select(NormalizeProduct).ToList();
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/Products/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var product = await response.Content.ReadFromJsonAsync<ProductDto>();
            return product is null ? null : NormalizeProduct(product);
        }

        public async Task<IEnumerable<ProductDto>> GetFeaturedAsync()
        {
            var products = await _httpClient.GetFromJsonAsync<IEnumerable<ProductDto>>("api/Products/featured") ?? Enumerable.Empty<ProductDto>();
            return products.Select(NormalizeProduct).ToList();
        }

        public async Task<IEnumerable<ProductDto>> GetByCategoryAsync(int categoryId)
        {
            var products = await _httpClient.GetFromJsonAsync<IEnumerable<ProductDto>>($"api/Products/category/{categoryId}") ?? Enumerable.Empty<ProductDto>();
            return products.Select(NormalizeProduct).ToList();
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Products", dto);
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException(await ReadErrorAsync(response, "Não foi possível cadastrar o produto."));
            return (await response.Content.ReadFromJsonAsync<ProductDto>())!;
        }

        public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Products/{id}", dto);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException(await ReadErrorAsync(response, "Não foi possível atualizar o produto."));
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

        private static ProductDto NormalizeProduct(ProductDto product)
        {
            product.ImageUrls = product.ImageUrls?.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).ToList() ?? new List<string>();
            if (product.ImageUrls.Count == 0 && !string.IsNullOrWhiteSpace(product.ImageUrl))
                product.ImageUrls.Add(product.ImageUrl!);

            // Corrige também produtos antigos que ainda possuem P/M/G salvo em um produto de calçado.
            // Produtos sem variação ficam com lista vazia; a View não inventa tamanho.
            product.AvailableSizes = ProductCatalogRules.NormalizeSizes(product.CategoryName, product.AvailableSizes);
            product.ColorImageUrls ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            return product;
        }

        private static async Task<string> ReadErrorAsync(HttpResponseMessage response, string fallback)
        {
            try
            {
                var error = await response.Content.ReadFromJsonAsync<ApiError>();
                return string.IsNullOrWhiteSpace(error?.Message) ? fallback : error.Message;
            }
            catch
            {
                return fallback;
            }
        }

        private sealed class ApiError { public string Message { get; set; } = string.Empty; }
    }
}
