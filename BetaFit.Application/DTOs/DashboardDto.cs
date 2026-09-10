// =============================================================================

// BetaFit.Application - DTO DashboardDto

// =============================================================================

namespace BetaFit.Application.DTOs

{

    /// <summary>

    /// DTO com as métricas resumidas exibidas no Dashboard administrativo

    /// (Website e Desktop).

    /// </summary>

    public class DashboardDto

    {

        public int TotalProducts { get; set; }

        public int TotalCategories { get; set; }

        public int FeaturedProducts { get; set; }

        public int ActiveProducts { get; set; }

        public IEnumerable<ProductDto> RecentProducts { get; set; } = new List<ProductDto>();

    }

}
