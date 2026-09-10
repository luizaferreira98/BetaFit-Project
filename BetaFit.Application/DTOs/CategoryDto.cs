// =============================================================================
// BetaFit.Application - DTOs de Category
// =============================================================================

using System.ComponentModel.DataAnnotations;

namespace BetaFit.Application.DTOs
{
    /// <summary>
    /// DTO para transferência de dados de uma Categoria.
    /// </summary>
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Slug { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public int SortOrder { get; set; }
        public bool HideWhenOutOfStock { get; set; } = true;

        /// <summary>
        /// Quantidade de produtos nesta categoria.
        /// Útil para mostrar no dashboard e na listagem.
        /// </summary>
        public int ProductCount { get; set; }
        public int TotalStock { get; set; }
        public int ActiveProducts { get; set; }
        public int InactiveProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public int MonthSales { get; set; }
        public bool IsVisible { get; set; }
        public string? ParentName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO para criação de uma nova Categoria.
    /// </summary>
    public class CreateCategoryDto
    {
        [Required, StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Slug { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public int SortOrder { get; set; }
        public bool HideWhenOutOfStock { get; set; } = true;
    }

    /// <summary>
    /// DTO para atualização de uma Categoria existente.
    /// </summary>
    public class UpdateCategoryDto
    {
        [Required, StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Slug { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public int SortOrder { get; set; }
        public bool HideWhenOutOfStock { get; set; } = true;
    }
}
