using System.Net.Http.Json;
using BetaFit.Application.DTOs;

namespace BetaFit.UI.Services;

public class HttpCartService
{
    private readonly HttpClient _http;
    public HttpCartService(HttpClient http) => _http = http;
    public async Task<List<CartItemDto>> GetAsync()
        => await _http.GetFromJsonAsync<List<CartItemDto>>("api/cart") ?? new();
    public async Task<(bool Ok,string Message)> AddAsync(int productId,int quantity,string? size,string? color)
    {
        var r=await _http.PostAsJsonAsync("api/cart",new AddCartItemDto{ProductId=productId,Quantity=quantity,Size=size,Color=color});
        return await Result(r,"Não foi possível adicionar o produto ao carrinho.");
    }
    public async Task<(bool Ok,string Message)> UpdateAsync(int productId,string? size,string? color,int quantity)
    {
        var r=await _http.PutAsJsonAsync($"api/cart/{productId}",new AddCartItemDto{ProductId=productId,Quantity=quantity,Size=size,Color=color});
        return await Result(r,"Não foi possível atualizar o carrinho.");
    }
    public async Task<bool> RemoveAsync(int productId,string? size,string? color)
        => (await _http.DeleteAsync($"api/cart/{productId}?size={Uri.EscapeDataString(size??string.Empty)}&color={Uri.EscapeDataString(color??string.Empty)}")).IsSuccessStatusCode;
    public async Task<bool> ClearAsync() => (await _http.DeleteAsync("api/cart")).IsSuccessStatusCode;

    private static async Task<(bool Ok,string Message)> Result(HttpResponseMessage r,string fallback)
    {
        if(r.IsSuccessStatusCode) return (true,string.Empty);
        try { var e=await r.Content.ReadFromJsonAsync<ApiError>(); return (false,e?.Message??fallback); }
        catch { return (false,fallback); }
    }
    private sealed class ApiError { public string Message {get;set;}=string.Empty; }
}
