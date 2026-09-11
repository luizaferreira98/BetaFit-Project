using System.Net.Http.Json;
using BetaFit.Application.DTOs;

namespace BetaFit.UI.Services;

public sealed class HttpSiteSettingsService
{
    private readonly HttpClient _http;
    public HttpSiteSettingsService(HttpClient http) => _http = http;
    public async Task<SiteSettingsDto> GetAsync() => await _http.GetFromJsonAsync<SiteSettingsDto>("api/site-settings") ?? new();
    public async Task<bool> UpdateAsync(SiteSettingsDto dto) => (await _http.PutAsJsonAsync("api/site-settings", dto)).IsSuccessStatusCode;
}
