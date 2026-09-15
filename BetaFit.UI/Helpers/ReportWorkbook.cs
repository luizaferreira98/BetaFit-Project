using System.Globalization;
using System.IO.Compression;
using System.Xml.Linq;
using BetaFit.Application.DTOs;

namespace BetaFit.UI.Helpers;

// XLSX com células de texto explícitas: nomes de produtos nunca viram fórmulas.
public static class ReportWorkbook
{
    private static readonly XNamespace Ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
    public static byte[] Create(StoreReportDto report)
    {
        var sheets = new (string Name, List<object[]> Rows)[]
        {
            ("Resumo", new() {
                new object[] { "BetaFit — relatório demonstrativo", "Valor" },
                new object[] { "Início", report.From.ToString("dd/MM/yyyy") },
                new object[] { "Fim (inclusive)", report.To.ToString("dd/MM/yyyy") },
                new object[] { "Pedidos criados", report.Orders }, new object[] { "Pedidos pagos elegíveis", report.PaidOrders },
                new object[] { "Faturamento de produtos após cupons (R$)", report.Revenue }, new object[] { "Frete recebido (R$)", report.ShippingRevenue }, new object[] { "Descontos de cupons (R$)", report.Discounts },
                new object[] { "Ticket médio (R$)", Math.Round(report.AverageTicket, 2) },
                new object[] { "Critério", "Data de criação do pedido. Faturamento e ranking: pagos, excluindo cancelados e pedidos em reembolso." },
                new object[] { "Produtos", "Valor bruto por produto antes do cupom; usa preços registrados no pedido." }
            }),
            ("Produtos mais pedidos", new() { new object[] { "ID", "Produto", "Unidades", "Pedidos pagos", "Valor antes do cupom (R$)" } }),
            ("Faturamento diário", new() { new object[] { "Data", "Pedidos pagos", "Produtos (R$)", "Cupons (R$)", "Frete (R$)" } }),
            ("Status dos pedidos", new() { new object[] { "Status atual", "Quantidade" } })
        };
        sheets[1].Rows.AddRange(report.Products.Select(p => new object[] { p.ProductId, p.Name, p.Quantity, p.Orders, p.Gross }));
        sheets[2].Rows.AddRange(report.Days.Select(d => new object[] { d.Date.ToString("dd/MM/yyyy"), d.Orders, d.Revenue, d.Discounts, d.ShippingRevenue }));
        sheets[3].Rows.AddRange(report.Statuses.Select(s => new object[] { s.Status, s.Orders }));
        using var buffer = new MemoryStream();
        using (var zip = new ZipArchive(buffer, ZipArchiveMode.Create, true))
        {
            XNamespace content = "http://schemas.openxmlformats.org/package/2006/content-types";
            XNamespace rel = "http://schemas.openxmlformats.org/package/2006/relationships";
            XNamespace docRel = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
            Put(zip, "[Content_Types].xml", new XElement(content + "Types",
                new XElement(content + "Default", new XAttribute("Extension", "rels"), new XAttribute("ContentType", "application/vnd.openxmlformats-package.relationships+xml")),
                new XElement(content + "Default", new XAttribute("Extension", "xml"), new XAttribute("ContentType", "application/xml")),
                new XElement(content + "Override", new XAttribute("PartName", "/xl/workbook.xml"), new XAttribute("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml")),
                sheets.Select((s, i) => new XElement(content + "Override", new XAttribute("PartName", $"/xl/worksheets/sheet{i + 1}.xml"), new XAttribute("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml")))));
            Put(zip, "_rels/.rels", new XElement(rel + "Relationships", new XElement(rel + "Relationship", new XAttribute("Id", "rId1"), new XAttribute("Type", docRel.NamespaceName + "/officeDocument"), new XAttribute("Target", "xl/workbook.xml"))));
            Put(zip, "xl/workbook.xml", new XElement(Ns + "workbook", new XAttribute(XNamespace.Xmlns + "r", docRel), new XElement(Ns + "sheets", sheets.Select((s, i) => new XElement(Ns + "sheet", new XAttribute("name", s.Name), new XAttribute("sheetId", i + 1), new XAttribute(docRel + "id", $"rId{i + 1}"))))));
            Put(zip, "xl/_rels/workbook.xml.rels", new XElement(rel + "Relationships", sheets.Select((s, i) => new XElement(rel + "Relationship", new XAttribute("Id", $"rId{i + 1}"), new XAttribute("Type", docRel.NamespaceName + "/worksheet"), new XAttribute("Target", $"worksheets/sheet{i + 1}.xml")))));
            for (var i = 0; i < sheets.Length; i++)
            {
                var rows = sheets[i].Rows;
                Put(zip, $"xl/worksheets/sheet{i + 1}.xml", new XElement(Ns + "worksheet",
                    new XElement(Ns + "cols", Enumerable.Range(1, rows[0].Length).Select(n => new XElement(Ns + "col", new XAttribute("min", n), new XAttribute("max", n), new XAttribute("width", n == 2 ? 55 : 30), new XAttribute("customWidth", 1)))),
                    new XElement(Ns + "sheetData", rows.Select((row, index) => new XElement(Ns + "row", new XAttribute("r", index + 1), row.Select((value, column) => Cell($"{(char)('A' + column)}{index + 1}", value)))))));
            }
        }
        return buffer.ToArray();
    }
    private static XElement Cell(string address, object value) => value is decimal or int
        ? new XElement(Ns + "c", new XAttribute("r", address), new XElement(Ns + "v", Convert.ToString(value, CultureInfo.InvariantCulture)))
        : new XElement(Ns + "c", new XAttribute("r", address), new XAttribute("t", "inlineStr"), new XElement(Ns + "is", new XElement(Ns + "t", value.ToString())));
    private static void Put(ZipArchive zip, string path, XElement xml)
    { using var stream = zip.CreateEntry(path).Open(); new XDocument(xml).Save(stream); }
}
