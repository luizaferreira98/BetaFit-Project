namespace BetaFit.Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal Subtotal => UnitPrice * Quantity;

        public virtual Order? Order { get; set; }

        public virtual Product? Product { get; set; }
    }
}