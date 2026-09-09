namespace BetaFit.Application.DTOs;

public sealed class DashboardDto
{
    public int TotalProducts { get; set; }
    public int TotalCategories { get; set; }
    public int FeaturedProducts { get; set; }
    public int ActiveProducts { get; set; }
    public int OutOfStockProducts { get; set; }
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int CompletedOrders { get; set; }
    public int TotalCustomers { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageOrderValue { get; set; }
    public IEnumerable<DashboardMonthDto> SalesByMonth { get; set; } = Array.Empty<DashboardMonthDto>();
    public IEnumerable<DashboardStatusDto> OrdersByStatus { get; set; } = Array.Empty<DashboardStatusDto>();
    public IEnumerable<ProductDto> RecentProducts { get; set; } = Array.Empty<ProductDto>();
}

public sealed class DashboardMonthDto
{
    public string Label { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int Orders { get; set; }
}

public sealed class DashboardStatusDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}
