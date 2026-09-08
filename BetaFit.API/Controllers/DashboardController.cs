// =============================================================================
// BetaFit.API - DashboardController
// =============================================================================
// Expõe as métricas resumidas usadas nos painéis administrativos
// (Website e Desktop).
//
// Endpoint:
// GET /api/dashboard  Retorna o resumo (total de produtos, categorias, etc.)
// =============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;

namespace BetaFit.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Retorna o resumo de métricas do sistema.
        /// GET /api/dashboard
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<DashboardDto>> GetSummary()
        {
            var summary = await _dashboardService.GetSummaryAsync();
            return Ok(summary);
        }
    }
}