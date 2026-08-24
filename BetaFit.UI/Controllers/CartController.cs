// =============================================================================
// BetaFit.UI - CartController
// =============================================================================
// Carrinho simples baseado em Session — sem checkout real, apenas
// demonstrativo, igual ao front-end original da Beta Fit.
// URL: /Cart
// =============================================================================


using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;
using BetaFit.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

using BetaFit.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetaFit.UI.Controllers
{
    [Authorize]
    [Route("Cart")]
    public class CartController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Title"] = "Carrinho";
            ViewData["Total"] = CartService.Total(HttpContext);
            ViewData["ItemCount"] = CartService.ItemCount(HttpContext);
            return View(CartService.Get(HttpContext));
        }

        [HttpPost("UpdateQuantity")]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            quantity = Math.Clamp(quantity, 0, 99);
            CartService.UpdateQuantity(HttpContext, productId, quantity);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Remove")]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int productId)
        {
            CartService.Remove(HttpContext, productId);
            return RedirectToAction(nameof(Index));
        }

        private readonly IOrderService _orderService;
        public CartController(IOrderService orderService) => _orderService = orderService; // injete no construtor

        [HttpPost("Checkout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            var cart = CartService.Get(HttpContext);
            if (!cart.Any())
                return RedirectToAction(nameof(Index));

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = new CreateOrderDto
            {
                Items = cart.Select(c => new CreateOrderItemDto
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity
                }).ToList()
            };

            await _orderService.CreateAsync(dto, userId);
            CartService.Clear(HttpContext);

            TempData["Sucesso"] = "Pedido realizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

    }
}
