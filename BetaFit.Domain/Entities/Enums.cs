// =============================================================================
// BetaFit.Domain - Enum Gender
// =============================================================================
// Público-alvo do produto no catálogo Beta Fit.
// =============================================================================

namespace BetaFit.Domain.Enums
{
    /// <summary>
    /// Público-alvo do produto.
    /// </summary>
    /// Variacoes de genero para produtos no catálogo Beta Fit.
    public enum Gender
    {
        Unissex = 0,
        Masculino = 1,
        Feminino = 2
    }


    //Opcoes de Status de pedido para o sistema de pedidos Beta Fit.
    public enum OrderStatus
    {
        Pendente = 0,
        EmPreparacao = 1,
        Pronto = 2,
        Entregue = 3,
        Cancelado = 4,
        Confirmado = 5,
        Enviado = 6,
        Reembolso = 7
    }

}