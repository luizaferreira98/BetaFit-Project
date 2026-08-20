// =============================================================================
// BetaFit.Application - Interface IDashboardService
// =============================================================================

using BetaFit.Application.DTOs;

namespace BetaFit.Application.Interfaces
{
    /// <summary>
    /// Contrato do serviço de Dashboard.
    /// Reúne as métricas resumidas do sistema.
    /// </summary>
    public interface IDashboardService
    {
        Task<DashboardDto> GetSummaryAsync();
    }
}