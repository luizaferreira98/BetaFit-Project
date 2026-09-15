using BetaFit.Domain.Entities;
namespace BetaFit.Domain.Interfaces;
public interface IShippingRuleRepository
{
    Task<List<ShippingRule>> GetActiveAsync();
}
