using BetaFit.Desktop.DTOs;
using BetaFit.Desktop.Helpers;

namespace BetaFit.Desktop.Services
{
    /// <summary>
    /// Serviço responsável apenas pela leitura das informações do dashboard.
    /// Não possui operações de criação, edição ou exclusão.
    /// </summary>
    public class DashboardApiService
    {
        private readonly HttpClientHelper _http;

        public DashboardApiService()
        {
            _http = HttpClientHelper.Instance;
        }

        public async Task<DashboardResponseDto?> GetSummaryAsync()
        {
            return await _http.GetAsync<DashboardResponseDto>("/api/dashboard");
        }
    }
}
