namespace BetaFit.Application.DTOs;

public class StoreReportDto
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public int Orders { get; set; }
    public int PaidOrders { get; set; }
    public decimal ShippingRevenue { get; set; }
    public decimal Revenue { get; set; }
    public decimal Discounts { get; set; }
    public decimal AverageTicket => PaidOrders == 0 ? 0 : Revenue / PaidOrders;
    public List<ReportProductDto> Products { get; set; } = new();
    public List<ReportDayDto> Days { get; set; } = new();
    public List<ReportStatusDto> Statuses { get; set; } = new();
}
public class ReportProductDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = "";
    public int Quantity { get; set; }
    public int Orders { get; set; }
    public decimal Gross { get; set; }
}
public class ReportDayDto
{
    public DateTime Date { get; set; }
    public int Orders { get; set; }
    public decimal ShippingRevenue { get; set; }
    public decimal Revenue { get; set; }
    public decimal Discounts { get; set; }
}
public class ReportStatusDto
{
    public string Status { get; set; } = "";
    public int Orders { get; set; }
}
