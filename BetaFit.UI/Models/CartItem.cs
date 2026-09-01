namespace BetaFit.UI.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? Size { get; set; }
        public int Quantity { get; set; }
    }
}
