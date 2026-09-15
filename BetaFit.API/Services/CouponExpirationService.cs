using BetaFit.Infraestructure.Context;
using Microsoft.EntityFrameworkCore;

namespace BetaFit.API.Services;

// Também reconcilia cupons expirados enquanto a API esteve desligada.
public sealed class CouponExpirationService(IServiceScopeFactory scopes, ILogger<CouponExpirationService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(15));
        do
        {
            try
            {
                using var scope = scopes.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<BetaFitDbContext>();
                var now = DateTime.UtcNow;
                await db.DiscountCoupons.Where(x => x.Active && (x.ExpiresAt <= now || x.Used >= x.MaxUses))
                    .ExecuteUpdateAsync(s => s.SetProperty(x => x.Active, false), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { return; }
            catch (Exception ex) { logger.LogError(ex, "Falha ao desativar cupons vencidos; nova tentativa em 15 segundos."); }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
