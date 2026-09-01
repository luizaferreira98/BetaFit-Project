// =============================================================================
// BetaFit.Domain - Entidade Product
// =============================================================================
// Esta classe representa a entidade principal do sistema: um produto (Product).
// Ela pertence à camada de DOMÍNIO, que é responsável por definir as entidades
// e regras de negócio do sistema.
//
//  CONCEITO IMPORTANTE:
// A camada Domain NÃO depende de nenhuma outra camada.
// Ela é o "coração" da aplicação e define O QUE o sistema é.
// =============================================================================

using BetaFit.Domain.Enums;

namespace BetaFit.Domain.Entities
{
    /// <summary>
    /// Representa um produto do catálogo institucional da Beta Fit.
    /// O preço é demonstrativo: não existe carrinho, checkout ou pagamento real.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Identificador único do produto (chave primária).
        /// O Entity Framework gera automaticamente esse valor.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome do produto. Exemplo: "Camiseta Dry Fit".
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição detalhada do produto.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Preço demonstrativo do produto.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// URL da imagem do produto.
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>Lista de imagens adicionais armazenada como JSON.</summary>
        public string ImageUrlsJson { get; set; } = "[]";

        /// <summary>Tamanhos disponíveis armazenados como JSON.</summary>
        public string AvailableSizesJson { get; set; } = "[\"P\",\"M\",\"G\",\"GG\"]";

        /// <summary>
        /// Público-alvo do produto.
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>
        /// Indica se o produto está em destaque na página inicial.
        /// </summary>
        public bool IsFeatured { get; set; }

        /// <summary>
        /// Indica se o produto está ativo e visível no catálogo.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Data de criação do registro no banco de dados.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Chave estrangeira (FK) que relaciona o produto com uma categoria.
        ///  CONCEITO: Foreign Key - conecta duas tabelas no banco de dados.
        /// </summary>
        public int CategoryId { get; set; }

        // =====================================================================
        // NAVIGATION PROPERTY (Propriedade de Navegação)
        // =====================================================================
        //  CONCEITO IMPORTANTE:
        // Navigation Properties permitem que o Entity Framework carregue
        // automaticamente os dados relacionados de outra tabela.
        // Aqui, cada Product "navega" até sua Category correspondente.
        // =====================================================================

        /// <summary>
        /// Categoria à qual este produto pertence (propriedade de navegação).
        /// </summary>
        public virtual Category? Category { get; set; }
    }
}