namespace BetaFit.Domain.Interfaces;
public interface ICouponRepository { Task<(string Code,decimal Discount)> RedeemAsync(string code,string userId,decimal subtotal); }
