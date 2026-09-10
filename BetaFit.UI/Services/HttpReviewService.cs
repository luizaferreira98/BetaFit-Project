using System.Net.Http.Json;
using BetaFit.Application.DTOs;

namespace BetaFit.UI.Services;

public class HttpReviewService
{
    private readonly HttpClient _http;
    public HttpReviewService(HttpClient http)=>_http=http;
    public async Task<List<ReviewDto>> GetByOrderAsync(int orderId)=>await _http.GetFromJsonAsync<List<ReviewDto>>($"api/reviews/order/{orderId}")??new();
    public async Task<List<ReviewDto>> GetByProductAsync(int productId)=>await _http.GetFromJsonAsync<List<ReviewDto>>($"api/reviews/product/{productId}")??new();
    public async Task<(bool Ok,string Message)> CreateAsync(int orderId,int productId,CreateReviewDto dto)
    { var r=await _http.PostAsJsonAsync($"api/reviews/order/{orderId}/product/{productId}",dto); if(r.IsSuccessStatusCode)return(true,"Avaliação enviada com sucesso.");var e=await r.Content.ReadFromJsonAsync<ApiError>();return(false,e?.Message??"Não foi possível enviar a avaliação."); }
    public async Task<List<ReviewDto>> ModerationAsync()=>await _http.GetFromJsonAsync<List<ReviewDto>>("api/reviews/moderation")??new();
    public async Task<bool> ModerateAsync(int id,string status)=>(await _http.PutAsJsonAsync($"api/reviews/{id}/moderation",status)).IsSuccessStatusCode;
    private sealed class ApiError{public string Message{get;set;}=string.Empty;}
}
