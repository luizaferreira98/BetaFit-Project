using BetaFit.Application.DTOs;
namespace BetaFit.Application.Interfaces;
public interface IShippingService
{
    Task<List<ShippingOptionDto>> QuoteAsync(string? cep, decimal productsAfterDiscount);
}
