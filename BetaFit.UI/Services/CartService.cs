// =============================================================================
// BetaFit.UI - Services/CartService.cs
// =============================================================================
//  CONCEITO: Carrinho simples baseado em Sessão
// Assim como no front-end original da Beta Fit, o carrinho é apenas
// demonstrativo (sem checkout real) e vive na Session do usuário.
// =============================================================================

using System.Text.Json;
using BetaFit.UI.Models;

namespace BetaFit.UI.Services
{
    public static class CartService
    {
        private const string Key = "betafit_cart";

        public static List<CartItem> Get(HttpContext context)
        {
            var json = context.Session.GetString(Key);
            return string.IsNullOrWhiteSpace(json)
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(json) ?? new();
        }

        private static void Save(HttpContext context, List<CartItem> items)
            => context.Session.SetString(Key, JsonSerializer.Serialize(items));

        public static void Add(HttpContext context, BetaFit.Application.DTOs.ProductDto product)
        {
            var items = Get(context);
            var item = items.FirstOrDefault(x => x.ProductId == product.Id);
            if (item is null)
            {
                items.Add(new CartItem
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                    Quantity = 1
                });
            }
            else
            {
                item.Quantity++;
            }
            Save(context, items);
        }

        public static void Remove(HttpContext context, int productId)
        {
            var items = Get(context);
            items.RemoveAll(x => x.ProductId == productId);
            Save(context, items);
        }

        public static decimal Total(HttpContext context) => Get(context).Sum(x => x.Price * x.Quantity);

        public static int ItemCount(HttpContext context) => Get(context).Sum(x => x.Quantity);
    }
}
