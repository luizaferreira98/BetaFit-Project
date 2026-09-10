using BetaFit.Application.DTOs;
using BetaFit.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BetaFit.UI.Models
{
    /// <summary>
    /// ViewModel da página de Catálogo (MVC), equivalente ao par
    /// CatalogQuery + PagedResponse usado no front original da Beta Fit.
    /// Como a BetaFit.API não expõe busca/paginação no servidor, os filtros
    /// são aplicados aqui, na UI, sobre os produtos retornados pela API.
    /// </summary>
    public class CatalogViewModel
    {
        [StringLength(100)] public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public Gender? Gender { get; set; }
        public string? SortBy { get; set; }
        public string? Availability { get; set; }
        public string ViewMode { get; set; } = "grid";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 9;

        public IReadOnlyList<ProductDto> Items { get; set; } = Array.Empty<ProductDto>();
        public IReadOnlyList<CategoryDto> Categories { get; set; } = Array.Empty<CategoryDto>();
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
