namespace BetaFit.Domain.Entities;

public class ShippingRule
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string CepStart { get; set; } = "01000000";
    public string CepEnd { get; set; } = "99999999";
    public decimal Price { get; set; }
    public int MinDays { get; set; } = 5;
    public int MaxDays { get; set; } = 10;
    public decimal? FreeAbove { get; set; }
    public bool Active { get; set; } = true;
}
