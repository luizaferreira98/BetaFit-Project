// =============================================================================
// BetaFit.UI - Services/HttpCategoryService.cs
// =============================================================================

using System.Net.Http.Json;
using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;

namespace BetaFit.UI.Services
{
    public class HttpCategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;

        public HttpCategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CategoryDto>>("api/Categories") ?? new List<CategoryDto>();
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/Categories/{id}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<CategoryDto>();
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Categories", dto);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<CategoryDto>())!;
        }

        public async Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Categories/{id}", dto);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<CategoryDto>();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Categories/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<int> CountAsync()
        {
            var categories = await GetAllAsync();
            return categories.Count();
        }
    }
}
