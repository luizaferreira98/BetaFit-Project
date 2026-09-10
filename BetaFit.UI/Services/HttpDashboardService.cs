// =============================================================================
// BetaFit.UI - Services/HttpDashboardService.cs
// =============================================================================

using System.Net.Http.Json;
using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;

namespace BetaFit.UI.Services
{
    public class HttpDashboardService : IDashboardService
    {
        private readonly HttpClient _httpClient;

        public HttpDashboardService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DashboardDto> GetSummaryAsync()
        {
            return await _httpClient.GetFromJsonAsync<DashboardDto>("api/Dashboard") ?? new DashboardDto();
        }
    }
}
