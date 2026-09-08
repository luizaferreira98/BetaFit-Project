using BetaFit.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetaFit.UI.Controllers;

[Authorize, Route("Notifications")]
public sealed class NotificationsController : Controller
{
    private readonly HttpNotificationService _service;
    public NotificationsController(HttpNotificationService service)=>_service=service;
    [HttpPost("ReadAll"),ValidateAntiForgeryToken]
    public async Task<IActionResult> ReadAll(string? returnUrl){await _service.ReadAllAsync();return LocalRedirect(string.IsNullOrWhiteSpace(returnUrl)?"/":returnUrl);}
}
