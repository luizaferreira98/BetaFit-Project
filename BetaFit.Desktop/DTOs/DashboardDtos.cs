namespace BetaFit.Desktop.DTOs;

public sealed class DashboardDto
{
    public int TotalProducts { get; set; }
    public int TotalCategories { get; set; }
    public int FeaturedProducts { get; set; }
    public int ActiveProducts { get; set; }
    public IEnumerable<object> RecentProducts { get; set; } = Array.Empty<object>();
}
