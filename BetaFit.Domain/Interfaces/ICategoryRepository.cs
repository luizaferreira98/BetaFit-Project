// =============================================================================
// BetaFit.Domain - Interface ICategoryRepository
// =============================================================================
// Contrato do repositório de categorias.
// Segue o mesmo padrão do IProductRepository.
// =============================================================================

using BetaFit.Domain.Entities;

namespace BetaFit.Domain.Interfaces
{
    /// <summary>
    /// Contrato do repositório de Categorias.
    /// Define as operações disponíveis para acessar dados de categorias.
    /// </summary>
    public interface ICategoryRepository
    {
        /// <summary>
        /// Retorna todas as categorias do banco de dados.
        /// </summary>
        Task<IEnumerable<Category>> GetAllAsync();

        /// <summary>
        /// Busca uma categoria específica pelo seu Id.
        /// </summary>
        Task<Category?> GetByIdAsync(int id);

        /// <summary>
        /// Adiciona uma nova categoria ao banco de dados.
        /// </summary>
        Task AddAsync(Category category);

        /// <summary>
        /// Atualiza os dados de uma categoria existente.
        /// </summary>
        Task UpdateAsync(Category category);

        /// <summary>
        /// Remove uma categoria do banco de dados.
        /// </summary>
        Task DeleteAsync(int id);

        /// <summary>
        /// Retorna o total de categorias cadastradas.
        /// </summary>
        Task<int> CountAsync();
    }
}