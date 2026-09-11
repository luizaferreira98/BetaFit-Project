using BetaFit.Application.DTOs;
using BetaFit.Domain.Entities;
using BetaFit.Infraestructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BetaFit.API.Services;

public interface INotificationService
{
    Task PurchaseCreatedAsync(OrderDto order, CancellationToken ct = default);
    Task OrderStatusChangedAsync(OrderDto order, string status, CancellationToken ct = default);
    Task ProductCreatedAsync(ProductDto product, CancellationToken ct = default);
}

public sealed class NotificationService : INotificationService
{
    private readonly BetaFitDbContext _db;
    private readonly UserManager<IdentityUser> _users;
    private readonly IEmailSender _email;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(BetaFitDbContext db, UserManager<IdentityUser> users, IEmailSender email, ILogger<NotificationService> logger)
    { _db = db; _users = users; _email = email; _logger = logger; }

    public async Task PurchaseCreatedAsync(OrderDto order, CancellationToken ct = default)
    {
        var buyer = await _users.FindByIdAsync(order.UserId);
        await AddAsync(order.UserId, "Pedido recebido", $"Seu pedido #{order.Id} foi criado com sucesso.", $"/Orders/Details/{order.Id}", "Order", ct);
        var staff = (await _users.GetUsersInRoleAsync("Admin")).Concat(await _users.GetUsersInRoleAsync("Funcionario")).DistinctBy(x => x.Id).ToList();
        foreach (var person in staff) await AddAsync(person.Id, "Nova compra", $"O pedido #{order.Id} foi realizado por {order.UserName} no valor de {order.Total:C}.", $"/Orders/Details/{order.Id}", "Sale", ct, false);
        await _db.SaveChangesAsync(ct);
        if (!string.IsNullOrWhiteSpace(buyer?.Email)) await SafeEmailAsync(buyer.Email!, $"Beta Fit - pedido #{order.Id} recebido", $"<h2>Pedido recebido</h2><p>Seu pedido <strong>#{order.Id}</strong> foi registrado no valor de <strong>{order.Total:C}</strong>.</p>", ct);
        foreach (var person in staff.Where(x => !string.IsNullOrWhiteSpace(x.Email))) await SafeEmailAsync(person.Email!, $"Beta Fit - nova compra #{order.Id}", $"<h2>Nova compra</h2><p>{order.UserName} realizou o pedido <strong>#{order.Id}</strong>, total {order.Total:C}.</p>", ct);
    }

    public async Task OrderStatusChangedAsync(OrderDto order, string status, CancellationToken ct = default)
    {
        await AddAsync(order.UserId, "Pedido atualizado", $"O pedido #{order.Id} agora está com o status {status}.", $"/Orders/Details/{order.Id}", "Order", ct);
        await _db.SaveChangesAsync(ct);
        var buyer = await _users.FindByIdAsync(order.UserId);
        if (!string.IsNullOrWhiteSpace(buyer?.Email)) await SafeEmailAsync(buyer.Email!, $"Beta Fit - atualização do pedido #{order.Id}", $"<h2>Status atualizado</h2><p>Seu pedido <strong>#{order.Id}</strong> agora está <strong>{status}</strong>.</p>", ct);
    }

    public async Task ProductCreatedAsync(ProductDto product, CancellationToken ct = default)
    {
        var relatedIds = await _db.OrderItems.AsNoTracking().Where(x => x.Product != null && x.Product.CategoryId == product.CategoryId)
            .Select(x => x.Order!.UserId).Distinct().ToListAsync(ct);
        var recipients = await _users.Users.Where(x => x.Email != null).ToListAsync(ct);
        foreach (var person in recipients)
        {
            var related = relatedIds.Contains(person.Id);
            var message = related ? $"Novo produto relacionado às suas compras: {product.Name}." : $"Novidade na loja: {product.Name}.";
            await AddAsync(person.Id, related ? "Novidade para você" : "Novo produto", message, $"/Product/{product.Id}", "Product", ct, false);
        }
        await _db.SaveChangesAsync(ct);
        foreach (var person in recipients)
        {
            var related = relatedIds.Contains(person.Id);
            await SafeEmailAsync(person.Email!, related ? $"Beta Fit - uma novidade escolhida para você" : $"Beta Fit - novo produto: {product.Name}",
                $"<h2>{(related ? "Novo produto relacionado às suas compras" : "Novidade na Beta Fit")}</h2><p><strong>{product.Name}</strong> já está disponível por {product.Price:C}.</p>", ct);
        }
    }

    private Task AddAsync(string userId, string title, string message, string? link, string type, CancellationToken ct, bool save = true)
    {
        _db.UserNotifications.Add(new UserNotification { UserId = userId, Title = title, Message = message, LinkUrl = link, Type = type });
        return save ? _db.SaveChangesAsync(ct) : Task.CompletedTask;
    }

    private async Task SafeEmailAsync(string to, string subject, string html, CancellationToken ct)
    {
        try { await _email.SendAsync(to, subject, html, ct); }
        catch (Exception ex) { _logger.LogWarning(ex, "Não foi possível enviar a notificação por e-mail para {Email}.", to); }
    }
}
