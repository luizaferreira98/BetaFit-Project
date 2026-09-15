using BetaFit.Application.Services;
using BetaFit.Infraestructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaFit.API.Controllers;

[ApiController, Authorize(Roles = "Admin"), Route("api/reports")]
public class ReportsController(BetaFitDbContext db) : ControllerBase
{
    [HttpGet, ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Get(DateTime? from, DateTime? to)
    {
        var start = from?.Date ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var end = to?.Date ?? DateTime.Today;
        if (end < start || (end - start).TotalDays > 366 || end >= DateTime.MaxValue.Date)
            return BadRequest(new { message = "Escolha um período válido de até 367 dias." });
        var exclusiveEnd = end.AddDays(1);
        var orders = await db.Orders.AsNoTracking().Include(o => o.Items)
            .Where(o => o.CreatedAt >= start && o.CreatedAt < exclusiveEnd).ToListAsync();
        return Ok(StoreReport.Build(orders, start, end));
    }
}
