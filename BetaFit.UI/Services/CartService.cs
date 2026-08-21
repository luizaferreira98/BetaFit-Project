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
            if (string.IsNullOrWhiteSpace(json))
                return new List<CartItem>();

            try
            {
                var items = JsonSerializer.Deserialize<List<CartItem>>(json) ?? new();
                return items
                    .Where(x => x.ProductId > 0 && x.Quantity > 0)
                    .Select(x =>
                    {
                        x.Quantity = Math.Clamp(x.Quantity, 1, 99);
                        return x;
                    })
                    .ToList();
            }
            catch (JsonException)
            {
                // Sessão corrompida não deve derrubar a página do carrinho.
                context.Session.Remove(Key);
                return new List<CartItem>();
            }
        }

        private static void Save(HttpContext context, List<CartItem> items)
            => context.Session.SetString(Key, JsonSerializer.Serialize(items));

        public static void Add(HttpContext context, BetaFit.Application.DTOs.ProductDto product, int quantity = 1)
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
                    Quantity = Math.Clamp(quantity, 1, 99)
                });
            }
            else
            {
                item.Quantity = Math.Min(item.Quantity + Math.Clamp(quantity, 1, 99), 99);
            }
            Save(context, items);
        }

        public static void UpdateQuantity(HttpContext context, int productId, int quantity)
        {
            var items = Get(context);
            var item = items.FirstOrDefault(x => x.ProductId == productId);
            if (item is null) return;

            if (quantity <= 0)
                items.Remove(item);
            else
                item.Quantity = Math.Min(quantity, 99);

            Save(context, items);
        }

        public static void Clear(HttpContext context)
        {
            context.Session.Remove(Key);
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
