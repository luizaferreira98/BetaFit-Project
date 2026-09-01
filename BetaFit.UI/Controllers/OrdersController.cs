using BetaFit.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetaFit.UI.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET /Orders/MyOrders
        public async Task<IActionResult> MyOrders()
        {
            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var orders = await _orderService.GetByUserIdAsync(userId);
            return View(orders);
        }

        // GET /Orders/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            // Verifica se o pedido pertence ao usuário atual (segurança)
            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            if (order.UserId != userId && !(User?.IsInRole("Admin") ?? false))
                return Forbid();

            return View(order);
        }
    }
}
