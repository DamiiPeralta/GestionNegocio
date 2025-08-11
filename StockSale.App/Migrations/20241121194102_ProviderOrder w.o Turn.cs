using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockSale.App.Migrations
{
    /// <inheritdoc />
    public partial class ProviderOrderwoTurn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProviderOrders_Turns_TurnId",
                table: "ProviderOrders");

            migrationBuilder.DropIndex(
                name: "IX_ProviderOrders_TurnId",
                table: "ProviderOrders");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ProviderOrders");

            migrationBuilder.DropColumn(
                name: "TurnId",
                table: "ProviderOrders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ProviderOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TurnId",
                table: "ProviderOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProviderOrders_TurnId",
                table: "ProviderOrders",
                column: "TurnId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderOrders_Turns_TurnId",
                table: "ProviderOrders",
                column: "TurnId",
                principalTable: "Turns",
                principalColumn: "Id");
        }
    }
}
