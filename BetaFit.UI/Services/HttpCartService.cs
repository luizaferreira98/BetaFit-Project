using System.Net.Http.Json;
using BetaFit.Application.DTOs;

namespace BetaFit.UI.Services;

public class HttpCartService
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _context;
    private readonly BetaFit.Application.Interfaces.IProductService _products;
    private HttpContext Context => _context.HttpContext!;
    private bool Guest => Context.User.Identity?.IsAuthenticated != true;
    private const string GuestKey = "BetaFit.GuestCart.v2";
    private List<CartItemDto> ReadGuest() => System.Text.Json.JsonSerializer.Deserialize<List<CartItemDto>>(Context.Session.GetString(GuestKey) ?? "[]") ?? new();
    private void SaveGuest(List<CartItemDto> items) => Context.Session.SetString(GuestKey, System.Text.Json.JsonSerializer.Serialize(items));
    public async Task MergeGuestAsync()
    {
        if (Guest) return;
        var remaining = ReadGuest();
        foreach (var item in remaining.ToList())
        {
            var result = await AddAsync(item.ProductId, item.Quantity, item.Size, item.Color);
            if (result.Ok) { remaining.Remove(item); SaveGuest(remaining); }
        }
        if (remaining.Count > 0) Context.Items["GuestCartWarning"] = "Alguns itens não puderam ser transferidos. Confira o estoque e suas opções no catálogo.";
    }
    private static bool Match(CartItemDto x, int id, string? size, string? color) => x.ProductId == id && string.Equals(x.Size,size,StringComparison.OrdinalIgnoreCase) && string.Equals(x.Color,color,StringComparison.OrdinalIgnoreCase);
    public HttpCartService(HttpClient http, IHttpContextAccessor context, BetaFit.Application.Interfaces.IProductService products) { _http=http; _context=context; _products=products; }
    public async Task<List<CartItemDto>> GetAsync()
        { if (Guest) return ReadGuest(); await MergeGuestAsync(); return await _http.GetFromJsonAsync<List<CartItemDto>>("api/cart") ?? new(); }
    public async Task<(bool Ok,string Message)> AddAsync(int productId,int quantity,string? size,string? color)
    {
        if (Guest)
        {
            var p=await _products.GetByIdAsync(productId);
            if(p is null || !p.IsActive) return(false,"Produto indisponível.");
            if(p.AvailableSizes.Any() && !p.AvailableSizes.Contains(size??"",StringComparer.OrdinalIgnoreCase)) return(false,"Selecione um tamanho disponível.");
            if(p.AvailableColors.Any() && !p.AvailableColors.Contains(color??"",StringComparer.OrdinalIgnoreCase)) return(false,"Selecione uma cor disponível.");
            var items=ReadGuest(); var existing=items.FirstOrDefault(x=>Match(x,productId,size,color));
            if(quantity<1 || quantity>99 || items.Where(x=>x.ProductId==productId).Sum(x=>x.Quantity)+quantity>p.Stock || (existing?.Quantity??0)+quantity>99) return(false,"Quantidade indisponível em estoque.");
            if(existing is null) items.Add(new CartItemDto{ProductId=p.Id,Name=p.Name,Price=p.Price,ImageUrl=p.ColorImageUrls.GetValueOrDefault(color??"")??p.ImageUrl,Size=size,Color=color,Quantity=quantity});
            else existing.Quantity+=quantity;
            SaveGuest(items); return(true,"");
        }
        var r=await _http.PostAsJsonAsync("api/cart",new AddCartItemDto{ProductId=productId,Quantity=quantity,Size=size,Color=color});
        return await Result(r,"Não foi possível adicionar o produto ao carrinho.");
    }
    public async Task<(bool Ok,string Message)> UpdateAsync(int productId,string? size,string? color,int quantity)
    {
        if (Guest) {
            var items=ReadGuest(); var item=items.FirstOrDefault(x=>Match(x,productId,size,color)); if(item is null)return(false,"Item não encontrado.");
            var p=await _products.GetByIdAsync(productId);
            if(p is null || !p.IsActive || quantity<1 || quantity>99 || items.Where(x=>x.ProductId==productId && x!=item).Sum(x=>x.Quantity)+quantity>p.Stock)return(false,"Quantidade indisponível em estoque.");
            item.Quantity=quantity; item.Price=p.Price; SaveGuest(items); return(true,"");
        }
        var r=await _http.PutAsJsonAsync($"api/cart/{productId}",new AddCartItemDto{ProductId=productId,Quantity=quantity,Size=size,Color=color});
        return await Result(r,"Não foi possível atualizar o carrinho.");
    }
    public async Task<bool> RemoveAsync(int productId,string? size,string? color)
        { if(Guest){var items=ReadGuest();items.RemoveAll(x=>Match(x,productId,size,color));SaveGuest(items);return true;} return (await _http.DeleteAsync($"api/cart/{productId}?size={Uri.EscapeDataString(size??string.Empty)}&color={Uri.EscapeDataString(color??string.Empty)}")).IsSuccessStatusCode; }
    public async Task<bool> ClearAsync() { if(Guest){Context.Session.Remove(GuestKey);return true;} return (await _http.DeleteAsync("api/cart")).IsSuccessStatusCode; }

    private static async Task<(bool Ok,string Message)> Result(HttpResponseMessage r,string fallback)
    {
        if(r.IsSuccessStatusCode) return (true,string.Empty);
        try { var e=await r.Content.ReadFromJsonAsync<ApiError>(); return (false,e?.Message??fallback); }
        catch { return (false,fallback); }
    }
    private sealed class ApiError { public string Message {get;set;}=string.Empty; }
}
