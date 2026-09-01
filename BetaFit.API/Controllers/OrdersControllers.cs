using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BetaFit.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly Microsoft.AspNetCore.SignalR.IHubContext<BetaFit.API.Hubs.OrderHub> _hubContext;

        public OrdersController(IOrderService orderService,
            Microsoft.AspNetCore.SignalR.IHubContext<BetaFit.API.Hubs.OrderHub> hubContext)
        {
            _orderService = orderService;
            _hubContext = hubContext;
        }

        // GET /api/orders -> usado pelo Desktop (Admin vê todos)
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _orderService.GetAllAsync());

        // GET /api/orders/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            return order is null ? NotFound() : Ok(order);
        }

        // GET /api/orders/mine -> usado pela UI (histórico do cliente logado)
        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return Ok(await _orderService.GetByUserIdAsync(userId));
        }

        // POST /api/orders -> chamado pelo Checkout do carrinho
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            try
            {
                var created = await _orderService.CreateAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PATCH /api/orders/{id}/status -> Desktop altera o status
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var ok = await _orderService.UpdateStatusAsync(id, status);

            if (!ok)
                return BadRequest(new { message = "Status inválido." });

            // Se o status for Entregue, notifica o usuário via SignalR
            if (string.Equals(status, "Entregue", StringComparison.OrdinalIgnoreCase))
            {
                var order = await _orderService.GetByIdAsync(id);
                if (order != null)
                {
                    // Envia notificação para o usuário dono do pedido
                    await _hubContext.Clients.User(order.UserId)
                        .SendCoreAsync("OrderFinalized", new object[] { new { orderId = order.Id } }, default);
                }
            }

            return NoContent();
        }
    }
}