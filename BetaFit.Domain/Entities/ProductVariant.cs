namespace BetaFit.Domain.Entities;
public class ProductVariant { public string Size {get;set;}=""; public string Color {get;set;}=""; public string Sku {get;set;}=""; public int Stock {get;set;} }
public static class VariantInventory {
 public static List<ProductVariant> Read(Product p)=>System.Text.Json.JsonSerializer.Deserialize<List<ProductVariant>>(p.VariantsJson)??new();
 public static ProductVariant? Find(List<ProductVariant> rows,string? size,string? color)=>rows.FirstOrDefault(v=>string.Equals(v.Size,size??"",StringComparison.OrdinalIgnoreCase)&&string.Equals(v.Color,color??"",StringComparison.OrdinalIgnoreCase));
 public static void Change(Product p,string? size,string? color,int delta){var rows=Read(p);if(rows.Count>0){var v=Find(rows,size,color);if(v==null)throw new InvalidOperationException("Variação não cadastrada.");if(v.Stock+delta<0)throw new InvalidOperationException("Estoque insuficiente para a variação.");v.Stock+=delta;p.VariantsJson=System.Text.Json.JsonSerializer.Serialize(rows);p.Stock=rows.Sum(v=>v.Stock);}else {if(p.Stock+delta<0)throw new InvalidOperationException("Estoque insuficiente.");p.Stock+=delta;}}
}
