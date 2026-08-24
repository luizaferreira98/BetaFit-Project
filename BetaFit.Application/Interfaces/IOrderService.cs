
using BetaFit.Application.DTOs;

namespace BetaFit.Application.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetAllAsync();
        Task<OrderDto?> GetByIdAsync(int id);
        Task<OrderDto> CreateAsync(CreateOrderDto dto, string userId);
        Task<bool> UpdateStatusAsync(int id, string status);
        Task<IEnumerable<OrderDto>> GetByUserIdAsync(string userId);
    }
}