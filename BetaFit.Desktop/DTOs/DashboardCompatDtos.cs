namespace BetaFit.Desktop.DTOs;

// Compatibility DTOs to satisfy older service / UI expectations
// These types mirror the existing ProductResponseDto and DashboardDto

public class DashboardProductDto : ProductResponseDto
{
    // Inherits all properties from ProductResponseDto
}

public class DashboardResponseDto
{
    public int TotalProducts { get; set; }
    public int TotalCategories { get; set; }
    public int FeaturedProducts { get; set; }
    public int ActiveProducts { get; set; }
    public IEnumerable<DashboardProductDto> RecentProducts { get; set; } = Array.Empty<DashboardProductDto>();
}

// Alias for historical Portuguese naming used in some services
public class UsuarioResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public bool Ativo { get; set; } = true;
}
