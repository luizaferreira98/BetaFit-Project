// =============================================================================
// BetaFit.Application - DashboardService
// =============================================================================
// Reúne as métricas exibidas no painel administrativo (Website e Desktop).
// =============================================================================

using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;
using BetaFit.Domain.Interfaces;

namespace BetaFit.Application.Services
{
    /// <summary>
    /// Serviço de Dashboard — agrega contagens de produtos e categorias.
    /// </summary>
    public class DashboardService : IDashboardService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public DashboardService(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<DashboardDto> GetSummaryAsync()
        {
            var products = (await _productRepository.GetAllAsync()).ToList();
            var totalCategories = await _categoryRepository.CountAsync();

            return new DashboardDto
            {
                TotalProducts = products.Count,
                TotalCategories = totalCategories,
                FeaturedProducts = products.Count(p => p.IsFeatured),
                ActiveProducts = products.Count(p => p.IsActive),
                RecentProducts = products
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(5)
                    .Select(p => new ProductDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        ImageUrl = p.ImageUrl,
                        Gender = p.Gender,
                        CategoryId = p.CategoryId,
                        CategoryName = p.Category?.Name ?? string.Empty,
                        IsFeatured = p.IsFeatured,
                        IsActive = p.IsActive,
                        CreatedAt = p.CreatedAt
                    })
            };
        }
    }
}