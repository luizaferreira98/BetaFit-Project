namespace BetaFit.Domain.Entities
{
    public class DiscountCoupon
    {
        public int Id { get; set; }
        // Código único do cupom de desconto (ex: "SUMMER10").
        public string Code { get; set; } = "";

        // Percentual de desconto aplicado pelo cupom (ex: 10 para 10%).
        public decimal Percent { get; set; }

        // Valor mínimo de compra necessário para que o cupom seja aplicável.
        public decimal Minimum { get; set; }

        // Indica a data de expiração do cupom. Após essa data, o cupom não é mais válido.
        public DateTime ExpiresAt { get; set; }
        public bool Active { get; set; } = true;

        //Maximo de usuarios
        public int MaxUses { get; set; } = 100;

        // Quantidade de vezes que o cupom já foi utilizado.
        public int Used { get; set; }

        //Função de calculo de cupom
        public decimal Calculate(decimal subtotal, DateTime now)
        {
            if (!Active || ExpiresAt <= now || Used >= MaxUses)
                throw new InvalidOperationException("Cupom expirado, inativo ou esgotado.");

            if (subtotal < Minimum)
                throw new InvalidOperationException($"Este cupom exige compra mínima de {Minimum:C}.");

            if (Percent <= 0 || Percent > 100)
                throw new InvalidOperationException("Desconto inválido.");

            //Retorna o valor do desconto arredondado para 2 casas decimais
            return Math.Round(subtotal * Percent / 100, 2, MidpointRounding.AwayFromZero);

            /// <summary>
            /// MidpointRounding.AwayFromZero é uma opção de arredondamento no C# (usada em funções como Math.Round)
            /// que instrui o sistema a arredondar números terminados em .5 para o inteiro mais distante de zero em valor absoluto.
            /// </summary>
        }
    }
}