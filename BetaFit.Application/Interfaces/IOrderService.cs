
using BetaFit.Application.DTOs;

namespace BetaFit.Application.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetAllAsync();
        Task<OrderDto?> GetByIdAsync(int id);
        Task<OrderDto> CreateAsync(CreateOrderDto dto, string userId);
        Task<bool> UpdateStatusAsync(int id, string status);
        Task<(bool Ok, string Message)> CancelAsync(int id, string userId);
        Task<(bool Ok, string Message)> UpdateDeliveryAsync(int id, string userId, UpdateOrderDeliveryDto dto);
        Task<(bool Ok, string Message)> ConfirmDemoPaymentAsync(int id, string userId);
        Task<IEnumerable<OrderDto>> GetByUserIdAsync(string userId);
    }
}
