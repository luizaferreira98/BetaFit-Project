using System.Net.Http.Json;
using BetaFit.Application.DTOs;
namespace BetaFit.UI.Services;
public class HttpPaymentService
{ private readonly HttpClient _http; public HttpPaymentService(HttpClient http)=>_http=http; public async Task<(bool Ok,PaymentPreferenceDto? Payment,string Message)> CreatePreferenceAsync(int orderId){var r=await _http.PostAsJsonAsync("api/payments/preferences",orderId);if(r.IsSuccessStatusCode)return(true,await r.Content.ReadFromJsonAsync<PaymentPreferenceDto>(),string.Empty);var e=await r.Content.ReadFromJsonAsync<ApiError>();return(false,null,e?.Message??"Não foi possível iniciar o pagamento.");} private sealed class ApiError{public string Message{get;set;}=string.Empty;} }
