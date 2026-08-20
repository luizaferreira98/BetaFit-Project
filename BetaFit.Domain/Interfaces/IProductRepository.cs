// =============================================================================
// BetaFit.Domain - Interface IProductRepository
// =============================================================================
//  CONCEITO IMPORTANTE:
// Uma INTERFACE define um CONTRATO - ela diz O QUE deve ser feito,
// mas NÃO diz COMO fazer. A implementação fica em outra camada.
//
// Isso é fundamental na arquitetura em camadas:
// - O Domain DEFINE a interface (o contrato)
// - O Infrastructure IMPLEMENTA a interface (o código real)
// - Isso permite trocar a implementação sem alterar o resto do sistema
// =============================================================================

using BetaFit.Domain.Entities;

namespace BetaFit.Domain.Interfaces
{
    /// <summary>
    /// Contrato do repositório de Products.
    /// Define as operações disponíveis para acessar dados de produtos.
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Retorna todos os produtos do banco de dados.
        /// </summary>
        Task<IEnumerable<Product>> GetAllAsync();

        /// <summary>
        /// Busca um produto específico pelo seu Id.
        /// Retorna null se não encontrar.
        /// </summary>
        Task<Product?> GetByIdAsync(int id);

        /// <summary>
        /// Retorna apenas os produtos marcados como destaque (IsFeatured = true).
        /// </summary>
        Task<IEnumerable<Product>> GetFeaturedAsync();

        /// <summary>
        /// Retorna todos os produtos de uma categoria específica.
        /// </summary>
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);

        /// <summary>
        /// Adiciona um novo produto ao banco de dados.
        /// </summary>
        Task AddAsync(Product product);

        /// <summary>
        /// Atualiza os dados de um produto existente.
        /// </summary>
        Task UpdateAsync(Product product);

        /// <summary>
        /// Remove um produto do banco de dados.
        /// </summary>
        Task DeleteAsync(int id);

        /// <summary>
        /// Retorna o total de produtos cadastrados.
        /// </summary>
        Task<int> CountAsync();
    }
}