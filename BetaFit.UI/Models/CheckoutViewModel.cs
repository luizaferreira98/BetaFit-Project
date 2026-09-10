using System.ComponentModel.DataAnnotations;
using BetaFit.UI.Models;

namespace BetaFit.UI.Models;

public class CheckoutViewModel
{
    [StringLength(60)] public string? CouponCode {get;set;}
    [Required, RegularExpression(@"^\d{3}\.?\d{3}\.?\d{3}-?\d{2}$", ErrorMessage="Informe um CPF válido.")]
    public string Cpf { get; set; } = string.Empty;
    [Required, RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage="Informe um CEP válido.")]
    public string Cep { get; set; } = string.Empty;
    [Required, StringLength(180)] public string Street { get; set; } = string.Empty;
    [Required, StringLength(20)] public string Number { get; set; } = string.Empty;
    [StringLength(120)] public string? Complement { get; set; }
    [Required, StringLength(120)] public string Neighborhood { get; set; } = string.Empty;
    [Required, StringLength(120)] public string City { get; set; } = string.Empty;
    [Required, StringLength(2, MinimumLength=2)] public string State { get; set; } = string.Empty;
    [Required(ErrorMessage = "Escolha uma forma de pagamento.")]
    public string PaymentMethod { get; set; } = "Pix";
    public bool HasSavedAddress { get; set; }
    public List<BetaFit.Application.DTOs.SavedCardDto> SavedCards { get; set; } = new();
    public string? SelectedCardId { get; set; }
    [Range(1,12)] public int Installments { get; set; } = 1;
    public bool HasSavedCard { get; set; }
    public bool UseSavedCard { get; set; }
    public string? SavedCardLabel { get; set; }
    [StringLength(120)] public string? CardHolderName { get; set; }
    [StringLength(19)] public string? CardNumber { get; set; }
    [StringLength(5)] public string? CardExpiry { get; set; }
    [StringLength(4)] public string? CardSecurityCode { get; set; }
    public List<CartItem> Items { get; set; } = new();
    public decimal Total => Items.Sum(x => x.Price * x.Quantity);
}
