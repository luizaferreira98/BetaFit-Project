using BetaFit.Application.DTOs;
using BetaFit.UI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetaFit.UI.Controllers;

[Authorize(Roles = "Admin"), Route("Admin/Reports")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class AdminReportsController(IHttpClientFactory factory, ILogger<AdminReportsController> logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(DateTime? from, DateTime? to, string? format)
    {
        var start = from?.Date ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var end = to?.Date ?? DateTime.Today;
        var empty = new StoreReportDto { From = start, To = end };
        if (!ModelState.IsValid || end < start || (end - start).TotalDays > 366 || end >= DateTime.MaxValue.Date)
        { ViewData["ReportError"] = "Escolha um período válido de até 367 dias."; return View(empty); }
        try
        {
            var api = factory.CreateClient("ApiClient");
            var report = await api.GetFromJsonAsync<StoreReportDto>($"api/reports?from={start:yyyy-MM-dd}&to={end:yyyy-MM-dd}")
                ?? throw new HttpRequestException("Resposta vazia do relatório.");
            if (format == "excel")
                return File(ReportWorkbook.Create(report), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"BetaFit-relatorio-{start:yyyyMMdd}-{end:yyyyMMdd}.xlsx");
            return View(report);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
        { logger.LogError(ex, "Falha ao gerar relatório"); ViewData["ReportError"] = "Não foi possível carregar o relatório. Tente novamente."; return View(empty); }
    }
}
