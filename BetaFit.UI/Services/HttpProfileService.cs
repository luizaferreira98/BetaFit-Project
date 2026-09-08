using System.Net.Http.Json;
using BetaFit.Application.DTOs;

namespace BetaFit.UI.Services;

public class HttpProfileService
{
    private readonly HttpClient _http;
    public HttpProfileService(HttpClient http) => _http = http;

    public async Task<UserDto?> GetAsync()
    {
        var response = await _http.GetAsync("api/profile");
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<UserDto>() : null;
    }

    public async Task<(bool Ok, string Message)> SaveCheckoutAddressAsync(CheckoutAddressDto address)
    {
        var response = await _http.PutAsJsonAsync("api/profile/checkout-address", address);
        if (response.IsSuccessStatusCode) return (true, string.Empty);
        try
        {
            var error = await response.Content.ReadFromJsonAsync<ApiError>();
            return (false, error?.Message ?? "Não foi possível salvar o endereço no perfil.");
        }
        catch { return (false, "Não foi possível salvar o endereço no perfil."); }
    }

    public async Task<(bool Ok, string Message)> SaveCardAsync(PaymentCardDto card)
    {
        var response = await _http.PutAsJsonAsync("api/profile/card", card);
        if (response.IsSuccessStatusCode) return (true, string.Empty);
        try { var error = await response.Content.ReadFromJsonAsync<ApiError>(); return (false, error?.Message ?? "Não foi possível salvar o cartão."); }
        catch { return (false, "Não foi possível salvar o cartão."); }
    }

    private sealed class ApiError { public string Message { get; set; } = string.Empty; }
}
