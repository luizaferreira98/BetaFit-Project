// =============================================================================
// BetaFit.UI - CartController
// =============================================================================
// Carrinho simples baseado em Session — sem checkout real, apenas
// demonstrativo, igual ao front-end original da Beta Fit.
// URL: /Cart
// =============================================================================

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
    }
}
