using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BetaFit.Application.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? PaymentId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = "Demo";
        public string? CustomerCpf { get; set; }
        public string? ShippingCep { get; set; }
        public string? ShippingStreet { get; set; }
        public string? ShippingNumber { get; set; }
        public string? ShippingComplement { get; set; }
        public string? ShippingNeighborhood { get; set; }
        public string? ShippingCity { get; set; }
        public string? ShippingState { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public decimal Subtotal { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class CreateOrderDto
    {
        public List<CreateOrderItemDto> Items { get; set; } = new();
        [StringLength(40)]
        public string? PaymentMethod { get; set; }
        [StringLength(14)]
        public string? CustomerCpf { get; set; }
        [StringLength(9)]
        public string? ShippingCep { get; set; }
        [StringLength(180)]
        public string? ShippingStreet { get; set; }
        [StringLength(20)]
        public string? ShippingNumber { get; set; }
        [StringLength(120)]
        public string? ShippingComplement { get; set; }
        [StringLength(120)]
        public string? ShippingNeighborhood { get; set; }
        [StringLength(120)]
        public string? ShippingCity { get; set; }
        [StringLength(2)]
        public string? ShippingState { get; set; }
        [StringLength(4)]
        public string? CardLast4 { get; set; }
    }

    public class CreateOrderItemDto
    {
        public int ProductId { get; set; }
        [Range(1,99)]
        public int Quantity { get; set; }
        [StringLength(30)]
        public string? Size { get; set; }
        [StringLength(40)]
        public string? Color { get; set; }
    }

    public class UpdateOrderDeliveryDto
    {
        [Required, RegularExpression(@"^\d{5}-?\d{3}$")] public string ShippingCep { get; set; } = string.Empty;
        [Required, StringLength(180)] public string ShippingStreet { get; set; } = string.Empty;
        [Required, StringLength(20)] public string ShippingNumber { get; set; } = string.Empty;
        [StringLength(120)] public string? ShippingComplement { get; set; }
        [Required, StringLength(120)] public string ShippingNeighborhood { get; set; } = string.Empty;
        [Required, StringLength(120)] public string ShippingCity { get; set; } = string.Empty;
        [Required, StringLength(2, MinimumLength = 2)] public string ShippingState { get; set; } = string.Empty;
    }
}
