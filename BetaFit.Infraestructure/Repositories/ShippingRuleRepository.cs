using BetaFit.Domain.Entities;
using BetaFit.Domain.Interfaces;
using BetaFit.Infraestructure.Context;
using Microsoft.EntityFrameworkCore;
namespace BetaFit.Infraestructure.Repositories;
public class ShippingRuleRepository(BetaFitDbContext db) : IShippingRuleRepository
{
    public Task<List<ShippingRule>> GetActiveAsync() => db.ShippingRules.AsNoTracking().Where(r => r.Active).ToListAsync();
}
