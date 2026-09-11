namespace BetaFit.Domain.Entities;

public sealed class SiteSettings
{
    public int Id { get; set; }
    public string Announcement { get; set; } = "Frete grátis em compras acima de R$ 299,90";
    public string HeaderLinksJson { get; set; } = "[]";
    public string HeroSlidesJson { get; set; } = "[]";
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
