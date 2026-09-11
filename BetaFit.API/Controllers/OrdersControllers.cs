using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BetaFit.API.Services;

namespace BetaFit.API.Controllers
{
    [ApiController,OrderTransaction]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly Microsoft.AspNetCore.SignalR.IHubContext<BetaFit.API.Hubs.OrderHub> _hubContext;
        private readonly INotificationService _notifications;

        public OrdersController(IOrderService orderService,
            Microsoft.AspNetCore.SignalR.IHubContext<BetaFit.API.Hubs.OrderHub> hubContext,
            INotificationService notifications)
        {
            _orderService = orderService;
            _hubContext = hubContext;
            _notifications = notifications;
        }

        // GET /api/orders -> usado pelo Desktop (Admin vê todos)
        [HttpGet]
        [Authorize(Roles = "Admin,Funcionario,Estoquista")]
        public async Task<IActionResult> GetAll()
            => Ok(await _orderService.GetAllAsync());

        // GET /api/orders/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order is null) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (order.UserId != userId && !User.IsInRole("Admin") && !(User.IsInRole("Funcionario") || User.IsInRole("Estoquista"))) return Forbid();
            return Ok(order);
        }

        // GET /api/orders/mine -> usado pela UI (histórico do cliente logado)
        [HttpGet("mine")]
        [Authorize]
        public async Task<IActionResult> GetMine()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return Ok(await _orderService.GetByUserIdAsync(userId));
        }

        // POST /api/orders -> chamado pelo Checkout do carrinho
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            try
            {
                if (!IsValidDemoCpf(dto.CustomerCpf)) return BadRequest(new { message = "Informe um CPF de teste com 11 dígitos." });
                if (string.IsNullOrWhiteSpace(dto.ShippingStreet) || string.IsNullOrWhiteSpace(dto.ShippingNumber) || string.IsNullOrWhiteSpace(dto.ShippingNeighborhood) || string.IsNullOrWhiteSpace(dto.ShippingCity) || string.IsNullOrWhiteSpace(dto.ShippingState))
                    return BadRequest(new { message = "Informe o endereço completo para a entrega." });
                var created = await _orderService.CreateAsync(dto, userId);
                await _notifications.PurchaseCreatedAsync(created, HttpContext.RequestAborted);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id:int}/cancel")]
        [Authorize]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _orderService.CancelAsync(id, userId);
            return result.Ok ? NoContent() : BadRequest(new { message = result.Message });
        }

        [HttpPut("{id:int}/delivery")]
        [Authorize]
        public async Task<IActionResult> UpdateDelivery(int id, [FromBody] UpdateOrderDeliveryDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _orderService.UpdateDeliveryAsync(id, userId, dto);
            return result.Ok ? NoContent() : BadRequest(new { message = result.Message });
        }

        [HttpPost("{id:int}/confirm-demo-payment")]
        [Authorize]
        public async Task<IActionResult> ConfirmDemoPayment(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _orderService.ConfirmDemoPaymentAsync(id, userId);
            return result.Ok ? NoContent() : BadRequest(new { message = result.Message });
        }

        // PATCH /api/orders/{id}/status -> Desktop altera o status
        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = "Admin,Funcionario,Estoquista")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var ok = await _orderService.UpdateStatusAsync(id, status);

            if (!ok)
                return BadRequest(new { message = "Status inválido." });

            var updatedOrder = await _orderService.GetByIdAsync(id);
            if (updatedOrder is not null)
                await _notifications.OrderStatusChangedAsync(updatedOrder, status, HttpContext.RequestAborted);

            // Se o status for Entregue, notifica o usuário via SignalR
            if (string.Equals(status, "Entregue", StringComparison.OrdinalIgnoreCase))
            {
                var order = updatedOrder;
                if (order != null)
                {
                    // Envia notificação para o usuário dono do pedido
                    await _hubContext.Clients.User(order.UserId)
                        .SendCoreAsync("OrderFinalized", new object[] { new { orderId = order.Id } }, default);
                }
            }

            return NoContent();
        }

    private static bool IsValidDemoCpf(string? value)
    {
        var cpf = new string((value ?? string.Empty).Where(char.IsDigit).ToArray());
        return cpf.Length == 11;
    }
}
}
