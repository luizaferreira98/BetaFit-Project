using System.Net.Http.Json;
using BetaFit.Application.DTOs;

namespace BetaFit.UI.Services;

public sealed class HttpNotificationService
{
    private readonly HttpClient _http;
    public HttpNotificationService(HttpClient http) => _http = http;
    public async Task<List<NotificationDto>> GetAsync()
    {
        try { return await _http.GetFromJsonAsync<List<NotificationDto>>("api/notifications") ?? new(); }
        catch { return new(); }
    }
    public async Task ReadAllAsync() => await _http.PostAsync("api/notifications/read-all", null);
}
