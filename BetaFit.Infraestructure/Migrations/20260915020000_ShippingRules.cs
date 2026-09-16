using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace BetaFit.Infraestructure.Migrations;

public partial class ShippingRules : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(name: "ShippingCost", table: "Orders", type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m);
        migrationBuilder.AddColumn<string>(name: "ShippingMethod", table: "Orders", type: "nvarchar(100)", maxLength: 100, nullable: true);
        migrationBuilder.AddColumn<int>(name: "ShippingMinDays", table: "Orders", type: "int", nullable: true);
        migrationBuilder.AddColumn<int>(name: "ShippingMaxDays", table: "Orders", type: "int", nullable: true);
        migrationBuilder.AddColumn<int>(name: "ShippingRuleId", table: "Orders", type: "int", nullable: true);
        migrationBuilder.CreateTable(name: "ShippingRules", columns: table => new {
            Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
            Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            CepStart = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
            CepEnd = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
            Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
            MinDays = table.Column<int>(type: "int", nullable: false), MaxDays = table.Column<int>(type: "int", nullable: false),
            FreeAbove = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
            Active = table.Column<bool>(type: "bit", nullable: false)
        }, constraints: table => table.PrimaryKey("PK_ShippingRules", x => x.Id));
        migrationBuilder.InsertData(table: "ShippingRules", columns: new[] { "Id", "Name", "CepStart", "CepEnd", "Price", "MinDays", "MaxDays", "FreeAbove", "Active" },
            values: new object[] { 1, "Entrega padrão", "01000000", "99999999", 19.90m, 5, 10, 299.90m, true });
    }
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("ShippingRules");
        foreach (var column in new[] { "ShippingCost", "ShippingMethod", "ShippingMinDays", "ShippingMaxDays", "ShippingRuleId" }) migrationBuilder.DropColumn(column, "Orders");
    }
}
