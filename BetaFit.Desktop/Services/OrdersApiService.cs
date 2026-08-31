using BetaFit.Desktop.Helpers;
using BetaFit.Desktop.DTOs;

namespace BetaFit.Desktop.Services
{
    public class OrdersApiService
    {
        private readonly HttpClientHelper _http;
        public OrdersApiService() => _http = HttpClientHelper.Instance;

        public async Task<List<OrderResponseDto>> GetAllAsync()
        {
            var orders = await _http.GetAsync<List<OrderResponseDto>>("/api/orders");
            return orders ?? new List<OrderResponseDto>();
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateStatusAsync(int id, string status)
        {
            var (success, _, error) = await _http.PatchAsync<object>($"/api/orders/{id}/status", status);
            return (success, error);
        }
    }
}