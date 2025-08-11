using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockSale.App.Migrations
{
    /// <inheritdoc />
    public partial class Is_ClosedTurn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProviderOrders_Providers_ProviderId",
                table: "ProviderOrders");

            migrationBuilder.AddColumn<bool>(
                name: "Is_Closed",
                table: "Turns",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "ProviderId",
                table: "ProviderOrders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderOrders_Providers_ProviderId",
                table: "ProviderOrders",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProviderOrders_Providers_ProviderId",
                table: "ProviderOrders");

            migrationBuilder.DropColumn(
                name: "Is_Closed",
                table: "Turns");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProviderId",
                table: "ProviderOrders",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderOrders_Providers_ProviderId",
                table: "ProviderOrders",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "Id");
        }
    }
}
