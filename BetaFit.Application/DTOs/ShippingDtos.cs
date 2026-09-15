using System.ComponentModel.DataAnnotations;

namespace BetaFit.Application.DTOs;

public class ShippingRuleDto : IValidatableObject
{
    public int Id { get; set; }
    [Required, StringLength(100, MinimumLength = 2)] public string Name { get; set; } = "";
    [Required, RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "Informe um CEP com 8 dígitos.")] public string CepStart { get; set; } = "01000000";
    [Required, RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "Informe um CEP com 8 dígitos.")] public string CepEnd { get; set; } = "99999999";
    [Range(typeof(decimal), "0", "99999.99", ParseLimitsInInvariantCulture = true)] public decimal Price { get; set; }
    [Range(1, 90)] public int MinDays { get; set; } = 5;
    [Range(1, 90)] public int MaxDays { get; set; } = 10;
    [Range(typeof(decimal), "0", "999999.99", ParseLimitsInInvariantCulture = true)] public decimal? FreeAbove { get; set; }
    public bool Active { get; set; } = true;
    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (MinDays > MaxDays) yield return new ValidationResult("O prazo máximo deve ser maior ou igual ao mínimo.", new[] { nameof(MaxDays) });
        var start = CepStart?.Replace("-", ""); var end = CepEnd?.Replace("-", "");
        if (start == "00000000" || string.CompareOrdinal(start, end) > 0)
            yield return new ValidationResult("Confira a faixa de CEP: o inicial deve ser válido e menor ou igual ao final.", new[] { nameof(CepStart) });
    }
}
public class ShippingOptionDto
{
    public int RuleId { get; set; }
    public string Name { get; set; } = "";
    public decimal Cost { get; set; }
    public decimal RegularCost { get; set; }
    public int MinDays { get; set; }
    public int MaxDays { get; set; }
    public decimal? FreeAbove { get; set; }
    public decimal? RemainingForFree { get; set; }
}
public class ShippingQuoteRequest
{
    [StringLength(9)] public string? Cep { get; set; }
    [StringLength(40)] public string? CouponCode { get; set; }
    [Required, MinLength(1), MaxLength(200)] public List<CreateOrderItemDto> Items { get; set; } = new();
}
public class ShippingQuoteDto
{
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public string? CouponCode { get; set; }
    public string? CouponError { get; set; }
    public string? ShippingError { get; set; }
    public List<ShippingOptionDto> Options { get; set; } = new();
}
