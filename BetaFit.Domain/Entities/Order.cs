using BetaFit.Domain.Enums; 
namespace BetaFit.Domain.Entities

{
    public class Order
    {
        public int Id { get; set; }


        //Dados de boleto
        // Digitos do boleto, caso o pagamento seja via boleto bancário. Exemplo: "12345678901234567890".
        public string? BoletoDigits { get; set; }
        
        // Data de vencimento do boleto, caso o pagamento seja via boleto bancário.
        public DateTime? BoletoDueAt { get; set; }
        
        // Data de pagamento do boleto, caso o pagamento seja via boleto bancário.
        public int Installments { get; set; } = 1;

        // Código de rastreio do pedido, caso o pedido tenha sido enviado. Exemplo: "BR1234567890".
        public string? TrackingCode {get;set;}

        // Descrição do status de rastreio do pedido, caso o pedido tenha sido enviado. Exemplo: "Em trânsito", "Entregue", etc.
        public string? TrackingDescription {get;set;}

        // Data de entrega do pedido, caso o pedido tenha sido entregue. Exemplo: "2024-06-01 14:30:00".
        public DateTime? DeliveredAt {get;set;}

        // Data de envio do pedido, caso o pedido tenha sido enviado. Exemplo: "2024-05-30 10:00:00".
        public decimal? ExperienceRating {get;set;}

        // Comentário do usuário sobre a experiência de compra, caso o pedido tenha sido entregue. Exemplo: "Produto chegou no prazo e em bom estado.".
        public string? ExperienceComment {get;set;}

        // Código do cupom de desconto aplicado ao pedido, caso tenha sido utilizado. Exemplo: "SUMMER10".
        public string? ReviewCoupon {get;set;}

        // Nota de avaliação do usuário sobre o produto, caso o pedido tenha sido entregue. Exemplo: 4.5 (de 0 a 5).
        public string? CouponCode {get;set;}

        // Valor do desconto aplicado ao pedido, caso tenha sido utilizado um cupom de desconto. Exemplo: 10.00 (em reais).
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

        // Dados de rastreio do pedido, caso o pedido tenha sido enviado. Exemplo: "BR1234567890".
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public decimal ShippingCost { get; set; }
        public string? ShippingMethod { get; set; }
        public int? ShippingMinDays { get; set; }
        public int? ShippingMaxDays { get; set; }
        public int? ShippingRuleId { get; set; }
        public decimal Total { get; set; }


        // Status do pedido. Exemplo: "Pendente", "Pago", "Enviado", "Entregue", "Cancelado".
        public OrderStatus Status { get; set; } = OrderStatus.Pendente;


        // Dados de pagamento do pedido. Exemplo: "Pix", "Boleto", "Cartão de Crédito".
        public string? PaymentId { get; set; }
        public string PaymentMethod { get; set; } = "Pix demonstrativo";
        public string PaymentStatus { get; set; } = "Pending";


        // Dados de envio do pedido. Exemplo: "Sedex", "PAC", "Retirada na loja".
        public virtual ICollection<OrderItem> Items { get; set; }
            = new List<OrderItem>();
    }
}
