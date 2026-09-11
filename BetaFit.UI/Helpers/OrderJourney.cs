using BetaFit.Application.DTOs;
namespace BetaFit.UI.Helpers;
public static class OrderJourney {
 public static readonly (string Key,string Label)[] Tabs={ ("all","Tudo"),("pay","A pagar"),("preparing","Preparando"),("shipping","A caminho"),("done","Finalizado"),("cancelled","Cancelado"),("refund","Reembolso")};
 public static string Group(OrderDto o)=>o.Status switch{"Cancelado"=>"cancelled","Reembolso"=>"refund","Entregue"=>"done","Enviado"=>"shipping",_=>o.PaymentStatus.Contains("Pago",StringComparison.OrdinalIgnoreCase)?"preparing":"pay"};
 public static string Label(OrderDto o)=>Tabs.First(x=>x.Key==Group(o)).Label;
}
