namespace BetaFit.Application.DTOs;

public class PaymentPreferenceDto
{
    public int OrderId { get; set; }
    public string PreferenceId { get; set; } = string.Empty;
    public string CheckoutUrl { get; set; } = string.Empty;
}
