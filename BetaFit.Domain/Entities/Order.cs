using BetaFit.Domain.Enums; 
namespace BetaFit.Domain.Entities

{
    public class Order
    {
        public int Id { get; set; }
        public string? BoletoDigits { get; set; }
        public DateTime? BoletoDueAt { get; set; }
        public int Installments { get; set; } = 1;
        public string? TrackingCode {get;set;}
        public string? TrackingDescription {get;set;}
        public DateTime? DeliveredAt {get;set;}
        public int? ExperienceRating {get;set;}
        public string? ExperienceComment {get;set;}
        public string? ReviewCoupon {get;set;}
        public string? CouponCode {get;set;}
        public decimal Discount {get;set;}


        public string UserId { get; set; } = string.Empty;

        // Snapshot dos dados usados na compra. O histórico do pedido não depende do perfil atual.
        public string? CustomerCpf { get; set; }
        public string? ShippingCep { get; set; }
        public string? ShippingStreet { get; set; }
        public string? ShippingNumber { get; set; }
        public string? ShippingComplement { get; set; }
        public string? ShippingNeighborhood { get; set; }
        public string? ShippingCity { get; set; }
        public string? ShippingState { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public decimal Total { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pendente;

        public string? PaymentId { get; set; }
        public string PaymentMethod { get; set; } = "Pix demonstrativo";
        public string PaymentStatus { get; set; } = "Pending";

        public virtual ICollection<OrderItem> Items { get; set; }
            = new List<OrderItem>();
    }
}
