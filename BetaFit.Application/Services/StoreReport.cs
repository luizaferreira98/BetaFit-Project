using BetaFit.Application.DTOs;
using BetaFit.Domain.Entities;
using BetaFit.Domain.Enums;

namespace BetaFit.Application.Services;

public static class StoreReport
{
    public static StoreReportDto Build(IEnumerable<Order> source, DateTime from, DateTime to)
    {
        var orders = source.Where(o => o.CreatedAt >= from.Date && o.CreatedAt < to.Date.AddDays(1)).ToList();
        var paid = orders.Where(o => o.Status is not (OrderStatus.Cancelado or OrderStatus.Reembolso)
            && (o.PaymentStatus.StartsWith("Pago", StringComparison.OrdinalIgnoreCase)
                || o.PaymentStatus.Equals("Paid", StringComparison.OrdinalIgnoreCase)
                || o.PaymentStatus.Equals("approved", StringComparison.OrdinalIgnoreCase))).ToList();
        return new StoreReportDto
        {
            From = from.Date, To = to.Date, Orders = orders.Count, PaidOrders = paid.Count,
            ShippingRevenue = paid.Sum(o => o.ShippingCost), Revenue = paid.Sum(o => o.Total - o.ShippingCost), Discounts = paid.Sum(o => o.Discount),
            Products = paid.SelectMany(o => o.Items).GroupBy(i => i.ProductId)
                .Select(g => new ReportProductDto { ProductId = g.Key, Name = g.Last().ProductName,
                    Quantity = g.Sum(i => i.Quantity), Orders = g.Select(i => i.OrderId).Distinct().Count(),
                    Gross = g.Sum(i => i.UnitPrice * i.Quantity) })
                .OrderByDescending(p => p.Quantity).ThenBy(p => p.Name).ToList(),
            Days = paid.GroupBy(o => o.CreatedAt.Date).OrderBy(g => g.Key)
                .Select(g => new ReportDayDto { Date = g.Key, Orders = g.Count(), ShippingRevenue = g.Sum(o => o.ShippingCost), Revenue = g.Sum(o => o.Total - o.ShippingCost), Discounts = g.Sum(o => o.Discount) }).ToList(),
            Statuses = orders.GroupBy(o => o.Status).OrderBy(g => g.Key)
                .Select(g => new ReportStatusDto { Status = g.Key.ToString(), Orders = g.Count() }).ToList()
        };
    }
}
