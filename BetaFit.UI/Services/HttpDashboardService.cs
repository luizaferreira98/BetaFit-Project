using System.Net;
using System.Net.Http.Json;
using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;

namespace BetaFit.UI.Services;

public class HttpDashboardService : IDashboardService
{
    private readonly HttpClient _httpClient;
    public HttpDashboardService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<DashboardDto> GetSummaryAsync()
    {
        try
        {
            using var response = await _httpClient.GetAsync("api/Dashboard");
            if (response.StatusCode == HttpStatusCode.Unauthorized) throw new HttpRequestException("Sua sessão expirou. Faça login novamente.");
            if (response.StatusCode == HttpStatusCode.Forbidden) throw new HttpRequestException("Você não tem permissão para visualizar o dashboard.");
            if (!response.IsSuccessStatusCode) throw new HttpRequestException("Não foi possível carregar os indicadores do dashboard.");
            return await response.Content.ReadFromJsonAsync<DashboardDto>() ?? new DashboardDto();
        }
        catch (TaskCanceledException ex) { throw new HttpRequestException("A conexão demorou demais para responder.", ex); }
    }
}
