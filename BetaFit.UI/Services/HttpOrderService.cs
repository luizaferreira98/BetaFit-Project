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
        {
            var response = await _httpClient.GetAsync("api/orders/mine");
            if (!response.IsSuccessStatusCode) return Array.Empty<OrderDto>();
            return await response.Content.ReadFromJsonAsync<IEnumerable<OrderDto>>() ?? Array.Empty<OrderDto>();
        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto dto, string userId)
        {
            var response = await _httpClient.PostAsJsonAsync("api/orders", dto);
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException(await ReadErrorAsync(response, "Não foi possível registrar o pedido."));
            return (await response.Content.ReadFromJsonAsync<OrderDto>())!;
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            var response = await _httpClient.PatchAsJsonAsync($"api/orders/{id}/status", status);
            return response.IsSuccessStatusCode;
        }

        public async Task<(bool Ok, string Message)> CancelAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/orders/{id}/cancel", null);
            return response.IsSuccessStatusCode
                ? (true, string.Empty)
                : (false, await ReadErrorAsync(response, "Não foi possível cancelar o pedido."));
        }

        Task<(bool Ok, string Message)> IOrderService.CancelAsync(int id, string userId) => CancelAsync(id);

        public async Task<(bool Ok, string Message)> UpdateDeliveryAsync(int id, string userId, UpdateOrderDeliveryDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/orders/{id}/delivery", dto);
            return response.IsSuccessStatusCode ? (true, string.Empty) : (false, await ReadErrorAsync(response, "Não foi possível atualizar o endereço."));
        }

        public async Task<(bool Ok, string Message)> ConfirmDemoPaymentAsync(int id, string userId)
        {
            var response = await _httpClient.PostAsync($"api/orders/{id}/confirm-demo-payment", null);
            return response.IsSuccessStatusCode ? (true, string.Empty) : (false, await ReadErrorAsync(response, "Não foi possível confirmar o pagamento."));
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
