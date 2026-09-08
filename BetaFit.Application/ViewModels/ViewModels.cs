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
using System.ComponentModel.DataAnnotations;

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
        public Dictionary<string, IReadOnlyList<ProductDto>> ProductsByCategory { get; set; } = new();
        public SiteSettingsDto SiteSettings { get; set; } = new();
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
        [Required(ErrorMessage = "Informe o nome do produto.")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Informe a descrição do produto.")]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;
        [Range(0, 999999.99)]
        public decimal Price { get; set; }
        [Range(0, 100000, ErrorMessage = "Informe um estoque entre 0 e 100000.")]
        public int Stock { get; set; } = 999;
        public string? ImageUrl { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public List<string> KeepImageUrls { get; set; } = new();
        public List<string> AvailableSizes { get; set; } = new();
        public List<string> AvailableColors { get; set; } = new();
        public Dictionary<string, string> ColorImageUrls { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        public Gender Gender { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecione uma categoria.")]
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
