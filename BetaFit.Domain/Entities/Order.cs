using BetaFit.Domain.Enums; 
namespace BetaFit.Domain.Entities

{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public decimal Total { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pendente;

        public virtual ICollection<OrderItem> Items { get; set; }
            = new List<OrderItem>();
    }
}