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
    public enum Gender
    {
        Unissex = 0,
        Masculino = 1,
        Feminino = 2
    }


    public enum OrderStatus
    {
        Pendente = 0,
        EmPreparacao = 1,
        Pronto = 2,
        Entregue = 3,
        Cancelado = 4,
        Confirmado = 5,
        Enviado = 6
    }

}