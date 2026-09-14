namespace BetaFit.Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        // armazenar o preço unitário de um item como um valor de alta precisão.
        public decimal UnitPrice { get; set; }

        //Preco Original do produto, caso tenha sido aplicado algum desconto no pedido.
        public decimal? OriginalPrice {get;set;}
        public string? ImageUrl {get;set;}

        public int Quantity { get; set; }

        public string? Size { get; set; }

        public string? Color { get; set; }

        public decimal Subtotal => UnitPrice * Quantity;



        // Referência para a entidade Order, permitindo o acesso aos detalhes do pedido associado a este item.
        public virtual Order? Order { get; set; }

        // Referência para a entidade Product, permitindo o acesso aos detalhes do produto associado a este item.
        public virtual Product? Product { get; set; }
    }
}
