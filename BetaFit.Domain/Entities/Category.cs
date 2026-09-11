// =============================================================================
// BetaFit.Domain - Entidade Category
// =============================================================================
// Esta classe representa uma categoria de produtos no sistema.
// Exemplos: "Camisetas", "Leggings", "Suplementos", etc.
//
//  CONCEITO IMPORTANTE:
// Uma Category possui MUITOS Products (relação 1:N - um para muitos).
// Isso significa que cada Category pode ter vários Products associados.
// =============================================================================

namespace BetaFit.Domain.Entities
{
    /// <summary>
    /// Representa uma categoria de produtos.
    /// Uma categoria agrupa produtos do mesmo tipo.
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Identificador único da categoria (chave primária).
        /// </summary>
        public int Id { get; set; }
        public string Slug { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public int SortOrder { get; set; }
        public bool HideWhenOutOfStock { get; set; } = true;

        /// <summary>
        /// Nome da categoria. Exemplo: "Camisetas", "Leggings".
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição da categoria.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// URL da imagem de capa da categoria.
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Indica se a categoria está ativa e visível no catálogo.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Data de criação do registro no banco de dados.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // =====================================================================
        // NAVIGATION PROPERTY - Coleção de Products
        // =====================================================================
        //  CONCEITO:
        // Uma Category pode ter VÁRIOS Products associados (relação 1:N).
        // O ICollection<Product> representa essa coleção de produtos.
        // O Entity Framework usa essa propriedade para fazer JOINs automáticos.
        // =====================================================================

        /// <summary>
        /// Lista de produtos que pertencem a esta categoria (propriedade de navegação).
        /// </summary>
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}