using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetaFit.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class StorefrontExperience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ColorGalleriesJson",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "{}");

            migrationBuilder.AddColumn<bool>(
                name: "MeasurementsAreDemo",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "SizeMeasurementsJson",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "{}");

            migrationBuilder.AddColumn<string>(
                name: "ModerationStatus",
                table: "ProductReviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Pendente");

            migrationBuilder.AddColumn<int>(
                name: "Attempts",
                table: "PendingProfileChanges",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedAt",
                table: "PendingProfileChanges",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "BoletoDigits",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BoletoDueAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Installments",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorGalleriesJson",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MeasurementsAreDemo",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SizeMeasurementsJson",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ModerationStatus",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "Attempts",
                table: "PendingProfileChanges");

            migrationBuilder.DropColumn(
                name: "RequestedAt",
                table: "PendingProfileChanges");

            migrationBuilder.DropColumn(
                name: "BoletoDigits",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BoletoDueAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Installments",
                table: "Orders");
        }
    }
}
