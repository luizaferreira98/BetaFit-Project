using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;
using BetaFit.Domain.Interfaces;

namespace BetaFit.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IOrderRepository _orderRepository;

    public DashboardService(IProductRepository productRepository, ICategoryRepository categoryRepository, IOrderRepository orderRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _orderRepository = orderRepository;
    }

    public async Task<DashboardDto> GetSummaryAsync()
    {
        // O DbContext é scoped e não suporta operações concorrentes na mesma instância.
        // Mantemos as consultas sequenciais para evitar "A second operation was started...".
        var products = (await _productRepository.GetAllAsync()).ToList();
        var totalCategories = await _categoryRepository.CountAsync();
        var orders = (await _orderRepository.GetAllAsync()).ToList();
        var revenueOrders = orders.Where(o => !string.Equals(o.Status.ToString(), "Cancelado", StringComparison.OrdinalIgnoreCase));
        var totalRevenue = revenueOrders.Sum(o => o.Total);

        var today = DateTime.Today;
        var startMonth = new DateTime(today.Year, today.Month, 1).AddMonths(-5);
        var months = Enumerable.Range(0, 6)
            .Select(i => startMonth.AddMonths(i))
            .Select(month => new DashboardMonthDto
            {
                Label = month.ToString("MMM/yy", new System.Globalization.CultureInfo("pt-BR")),
                Revenue = revenueOrders.Where(o => o.CreatedAt.Year == month.Year && o.CreatedAt.Month == month.Month).Sum(o => o.Total),
                Orders = orders.Count(o => o.CreatedAt.Year == month.Year && o.CreatedAt.Month == month.Month)
            })
            .ToList();

        var statuses = orders
            .GroupBy(o => o.Status.ToString())
            .Select(g => new DashboardStatusDto { Status = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToList();

        return new DashboardDto
        {
            TotalProducts = products.Count,
            TotalCategories = totalCategories,
            FeaturedProducts = products.Count(p => p.IsFeatured),
            ActiveProducts = products.Count(p => p.IsActive),
            OutOfStockProducts = products.Count(p => p.Stock <= 0),
            TotalOrders = orders.Count,
            PendingOrders = orders.Count(p => string.Equals(p.Status.ToString(), "Pendente", StringComparison.OrdinalIgnoreCase)),
            CompletedOrders = orders.Count(p => string.Equals(p.Status.ToString(), "Entregue", StringComparison.OrdinalIgnoreCase)),
            TotalCustomers = orders.Select(p => p.UserId).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).Count(),
            TotalRevenue = totalRevenue,
            AverageOrderValue = !revenueOrders.Any() ? 0 : totalRevenue / revenueOrders.Count(),
            SalesByMonth = months,
            OrdersByStatus = statuses,
            RecentProducts = products
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .Select(p => new ProductDto
                {
                    Id = p.Id, Name = p.Name, Description = p.Description, Price = p.Price, ImageUrl = p.ImageUrl,
                    Gender = p.Gender, CategoryId = p.CategoryId, CategoryName = p.Category?.Name ?? string.Empty,
                    IsFeatured = p.IsFeatured, IsActive = p.IsActive, CreatedAt = p.CreatedAt
                })
                .ToList()
        };
    }
}
