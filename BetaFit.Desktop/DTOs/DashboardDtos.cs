using BetaFit.Domain.Enums;

namespace BetaFit.Desktop.DTOs
{
    /// <summary>
    /// Resumo exibido no dashboard administrativo do Desktop.
    /// Corresponde ao retorno de GET /api/dashboard.
    /// </summary>
    public class DashboardResponseDto
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int FeaturedProducts { get; set; }
        public int ActiveProducts { get; set; }
        public List<DashboardProductDto> RecentProducts { get; set; } = new();
    }

    public class DashboardProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public Gender Gender { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
