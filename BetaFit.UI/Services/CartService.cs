using System.Text.Json;
using BetaFit.Application.DTOs;
using BetaFit.UI.Models;

namespace BetaFit.UI.Services
{
    public static class CartService
    {
        private const string Key = "betafit_cart";
        public static List<CartItem> Get(HttpContext context)
        {
            var json = context.Session.GetString(Key);
            if (string.IsNullOrWhiteSpace(json)) return new();
            try { return (JsonSerializer.Deserialize<List<CartItem>>(json) ?? new()).Where(x => x.ProductId > 0 && x.Quantity > 0).Select(x => { x.Quantity = Math.Clamp(x.Quantity,1,99); return x; }).ToList(); }
            catch (JsonException) { context.Session.Remove(Key); return new(); }
        }
        private static void Save(HttpContext context, List<CartItem> items) => context.Session.SetString(Key, JsonSerializer.Serialize(items));
        public static void Add(HttpContext context, ProductDto product, int quantity = 1, string? size = null)
        {
            var items = Get(context);
            size = string.IsNullOrWhiteSpace(size) ? product.AvailableSizes.FirstOrDefault() : size.Trim();
            if (product.AvailableSizes.Any() && !product.AvailableSizes.Contains(size!, StringComparer.OrdinalIgnoreCase)) throw new ArgumentException("Tamanho inválido.");
            var item = items.FirstOrDefault(x => x.ProductId == product.Id && string.Equals(x.Size, size, StringComparison.OrdinalIgnoreCase));
            if (item is null) items.Add(new CartItem { ProductId=product.Id, Name=product.Name, Price=product.Price, ImageUrl=product.ImageUrl, Size=size, Quantity=Math.Clamp(quantity,1,99) });
            else item.Quantity = Math.Min(item.Quantity + Math.Clamp(quantity,1,99), 99);
            Save(context, items);
        }
        public static void UpdateQuantity(HttpContext context, int productId, string? size, int quantity)
        {
            var items=Get(context); var item=items.FirstOrDefault(x=>x.ProductId==productId && string.Equals(x.Size,size,StringComparison.OrdinalIgnoreCase)); if(item is null)return;
            if(quantity<=0)items.Remove(item); else item.Quantity=Math.Min(quantity,99); Save(context,items);
        }
        public static void Remove(HttpContext context,int productId,string? size){var items=Get(context);items.RemoveAll(x=>x.ProductId==productId&&string.Equals(x.Size,size,StringComparison.OrdinalIgnoreCase));Save(context,items);}
        public static void Clear(HttpContext context)=>context.Session.Remove(Key);
        public static decimal Total(HttpContext context)=>Get(context).Sum(x=>x.Price*x.Quantity);
        public static int ItemCount(HttpContext context)=>Get(context).Sum(x=>x.Quantity);
    }
}
