using BetaFit.Domain.Interfaces;
using BetaFit.Infraestructure.Context;
using Microsoft.EntityFrameworkCore;
namespace BetaFit.Infraestructure.Repositories;
public class CouponRepository(BetaFitDbContext db):ICouponRepository {
 public async Task<(string Code,decimal Discount)> RedeemAsync(string code,string userId,decimal subtotal){
  var normalized=code.Trim().ToUpperInvariant();var coupon=await db.DiscountCoupons.SingleOrDefaultAsync(x=>x.Code==normalized)??throw new InvalidOperationException("Cupom não encontrado.");
  if(await db.Orders.AnyAsync(x=>x.UserId==userId&&x.CouponCode==normalized))throw new InvalidOperationException("Você já utilizou este cupom.");
  var discount=coupon.Calculate(subtotal,DateTime.UtcNow);coupon.Used++;if(coupon.Used>=coupon.MaxUses)coupon.Active=false;await db.SaveChangesAsync();return(coupon.Code,discount);
 }
}
