// =============================================================================
// BetaFit.Application - ViewModels
// =============================================================================
//  CONCEITO IMPORTANTE: ViewModels
// Um ViewModel é um objeto criado especificamente para uma View (tela).
// Ele contém EXATAMENTE os dados que aquela tela precisa exibir.
//
// Diferença entre DTO e ViewModel:
// - DTO: transferência genérica de dados entre camadas
// - ViewModel: dados específicos para uma tela/view
// =============================================================================

using BetaFit.Application.DTOs;
using BetaFit.Domain.Enums;

namespace BetaFit.Application.ViewModels
{

    /// <summary>
    /// ViewModel da página inicial (Home) do site institucional.
    /// Contém os produtos em destaque e as categorias para exibição.
    /// </summary>
    public class HomeViewModel
    {
        public IEnumerable<ProductDto> FeaturedProducts { get; set; } = new List<ProductDto>();
        public IEnumerable<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public IEnumerable<ProductDto> RecentProducts { get; set; } = new List<ProductDto>();
    }

    /// <summary>
    /// ViewModel da página de detalhes de um produto.
    /// </summary>
    public class ProductDetailsViewModel
    {
        public ProductDto Product { get; set; } = new ProductDto();
        public IEnumerable<ProductDto> RelatedProducts { get; set; } = new List<ProductDto>();
    }

    /// <summary>
    /// ViewModel do Dashboard administrativo.
    /// Contém as métricas resumidas do sistema.
    /// </summary>
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int FeaturedProducts { get; set; }
        public int ActiveProducts { get; set; }
        public IEnumerable<ProductDto> RecentProducts { get; set; } = new List<ProductDto>();
    }

    /// <summary>
    /// ViewModel para o formulário de criação/edição de produtos.
    /// Inclui a lista de categorias para o select/dropdown.
    /// </summary>
    public class ProductFormViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public List<string> KeepImageUrls { get; set; } = new();
        public List<string> AvailableSizes { get; set; } = new();

        public Gender Gender { get; set; }

        public Enum Enum { get; set; }

        public int CategoryId { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; }

        /// <summary>
        /// Lista de categorias disponíveis para o dropdown do formulário.
        /// </summary>
        public IEnumerable<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
    }

    /// <summary>
    /// ViewModel para o catálogo de produtos, com filtro por categoria/gênero.
    /// </summary>
    public class ProductListViewModel
    {
        public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
        public IEnumerable<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public int? SelectedCategoryId { get; set; }

        public Gender? SelectedGender { get; set; }
    }

    public class OrderViewModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<OrderItemViewModel> Items { get; set; } = new();
    }

    public class OrderItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }

        public Enum? SelectedEnum { get; set; }

    }
}