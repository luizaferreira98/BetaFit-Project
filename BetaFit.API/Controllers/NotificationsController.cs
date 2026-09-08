using System.Security.Claims;
using BetaFit.Application.DTOs;
using BetaFit.Infraestructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetaFit.API.Controllers;

[ApiController, Route("api/notifications"), Authorize]
public sealed class NotificationsController : ControllerBase
{
    private readonly BetaFitDbContext _db;
    public NotificationsController(BetaFitDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var items = await _db.UserNotifications.AsNoTracking().Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedAt).Take(30)
            .Select(x => new NotificationDto { Id=x.Id,Title=x.Title,Message=x.Message,LinkUrl=x.LinkUrl,Type=x.Type,IsRead=x.IsRead,CreatedAt=x.CreatedAt }).ToListAsync();
        return Ok(items);
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> ReadAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _db.UserNotifications.Where(x => x.UserId == userId && !x.IsRead).ExecuteUpdateAsync(s => s.SetProperty(x => x.IsRead, true));
        return NoContent();
    }
}
