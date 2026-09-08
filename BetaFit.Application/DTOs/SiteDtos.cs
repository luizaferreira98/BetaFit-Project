using System.ComponentModel.DataAnnotations;

namespace BetaFit.Application.DTOs;

public sealed class HeaderLinkDto
{
    [StringLength(40)] public string Label { get; set; } = string.Empty;
    [StringLength(300)] public string Url { get; set; } = string.Empty;
}

public sealed class HeroSlideDto
{
    [StringLength(80)] public string Title { get; set; } = string.Empty;
    [StringLength(180)] public string Subtitle { get; set; } = string.Empty;
    [StringLength(300)] public string MediaUrl { get; set; } = string.Empty;
    [StringLength(20)] public string MediaType { get; set; } = "image";
    [StringLength(40)] public string ButtonText { get; set; } = "Explorar coleção";
    [StringLength(300)] public string ButtonUrl { get; set; } = "/Catalog";
}

public sealed class SiteSettingsDto
{
    [StringLength(160)] public string Announcement { get; set; } = "Frete grátis em compras acima de R$ 299,90";
    public List<HeaderLinkDto> HeaderLinks { get; set; } = new();
    public List<HeroSlideDto> HeroSlides { get; set; } = new();
}

public sealed class NotificationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? LinkUrl { get; set; }
    public string Type { get; set; } = "Info";
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
