using System.Net.Http.Json;
using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;

namespace BetaFit.UI.Services
{
    public class HttpOrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        public HttpOrderService(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<List<OrderDto>> GetAllAsync()
            => await _httpClient.GetFromJsonAsync<List<OrderDto>>("api/orders") ?? new List<OrderDto>();

        public async Task<OrderDto?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/orders/{id}");
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<OrderDto>() : null;
        }

        public async Task<IEnumerable<OrderDto>> GetByUserIdAsync(string userId)
            => await _httpClient.GetFromJsonAsync<IEnumerable<OrderDto>>("api/orders/mine") ?? new List<OrderDto>();

        public async Task<OrderDto> CreateAsync(CreateOrderDto dto, string userId)
        {
            var response = await _httpClient.PostAsJsonAsync("api/orders", dto);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<OrderDto>())!;
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            var response = await _httpClient.PatchAsJsonAsync($"api/orders/{id}/status", status);
            return response.IsSuccessStatusCode;
        }
    }
}