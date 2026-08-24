// =============================================================================
// BetaFit.Domain - Interface IOrderRepository
// =============================================================================
// CONTRATO DO REPOSITÓRIO DE PEDIDOS.
//
// O Domain define O QUE o repositório deve fazer.
// A implementação fica na camada Infrastructure.
//
// Domain
//    ↓
// Define IOrderRepository
//
// Infrastructure
//    ↓
// Implementa OrderRepository
// =============================================================================

using BetaFit.Domain.Entities;
using BetaFit.Domain.Enums;

namespace BetaFit.Domain.Interfaces
{
    /// <summary>
    /// Contrato do repositório de Orders.
    /// Define as operações disponíveis para acessar pedidos.
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// Retorna todos os pedidos do banco de dados.
        /// </summary>
        Task<IEnumerable<Order>> GetAllAsync();

        /// <summary>
        /// Busca um pedido específico pelo seu Id.
        /// Retorna null caso não encontre.
        /// </summary>
        Task<Order?> GetByIdAsync(int id);

        /// <summary>
        /// Retorna todos os pedidos de um usuário específico (histórico do cliente).
        /// </summary>
        Task<IEnumerable<Order>> GetByUserIdAsync(string userId);

        /// <summary>
        /// Adiciona um novo pedido ao banco de dados.
        /// </summary>
        Task AddAsync(Order order);

        /// <summary>
        /// Atualiza apenas o status de um pedido existente.
        /// </summary>
        Task UpdateStatusAsync(int id, OrderStatus status);

        /// <summary>
        /// Atualiza os dados de um pedido existente.
        /// </summary>
        Task UpdateAsync(Order order);

        /// <summary>
        /// Remove um pedido do banco de dados.
        /// </summary>
        Task DeleteAsync(int id);

        /// <summary>
        /// Retorna o total de pedidos cadastrados.
        /// </summary>
        Task<int> CountAsync();
    }
}