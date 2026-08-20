// =============================================================================
// BetaFit.UI - CartController
// =============================================================================
// Carrinho simples baseado em Session — sem checkout real, apenas
// demonstrativo, igual ao front-end original da Beta Fit.
// URL: /Cart
// =============================================================================

using BetaFit.UI.Helpers;
using BetaFit.UI.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace BetaFit.UI.Controllers
{
    [Route("Cart")]
    public class CartController : Controller
    {
        [HttpGet]
        public IActionResult Index(int? remove)
        {
            ViewData["Title"] = "Carrinho";

            if (remove.HasValue)
            {
                CartService.Remove(HttpContext, remove.Value);
            }

            ViewData["Total"] = CartService.Total(HttpContext);
            return View(CartService.Get(HttpContext));
        }
    }
}
