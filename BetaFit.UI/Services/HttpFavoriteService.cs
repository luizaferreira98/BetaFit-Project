using System.Net.Http.Json;
using BetaFit.Application.DTOs;

namespace BetaFit.UI.Services;

public class HttpFavoriteService
{
    private readonly HttpClient _http;
    public HttpFavoriteService(HttpClient http)=>_http=http;
    public async Task<HashSet<int>> GetAsync()=> (await _http.GetFromJsonAsync<List<int>>("api/favorites")??new()).ToHashSet();
    public async Task<bool> IsFavoriteAsync(int productId)
    { var r=await _http.GetAsync($"api/favorites/{productId}"); if(!r.IsSuccessStatusCode)return false; return (await r.Content.ReadFromJsonAsync<FavoriteStatusDto>())?.IsFavorite==true; }
    public async Task<bool> AddAsync(int productId)=>(await _http.PostAsync($"api/favorites/{productId}",null)).IsSuccessStatusCode;
    public async Task<bool> RemoveAsync(int productId)=>(await _http.DeleteAsync($"api/favorites/{productId}")).IsSuccessStatusCode;
    public async Task<bool> ToggleAsync(int productId)
    { var current=await IsFavoriteAsync(productId); return current ? !await RemoveAsync(productId) : await AddAsync(productId); }
}
