namespace BetaFit.UI.Models
{
    /// <summary>
    /// Item do carrinho de compras (demonstrativo, guardado na Session).
    /// </summary>
    public class CartItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
    }
}
