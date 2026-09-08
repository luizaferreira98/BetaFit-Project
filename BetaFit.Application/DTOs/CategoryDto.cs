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

        /// <summary>
        /// Quantidade de produtos nesta categoria.
        /// Útil para mostrar no dashboard e na listagem.
        /// </summary>
        public int ProductCount { get; set; }
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
    }

    /// <summary>
    /// DTO para atualização de uma Categoria existente.
    /// </summary>
    public class UpdateCategoryDto
    {
        [Required, StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
