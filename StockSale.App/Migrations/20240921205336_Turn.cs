using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockSale.App.Migrations
{
    /// <inheritdoc />
    public partial class Turn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProviderOrderTurn");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProviderOrders_Turns_TurnId",
                table: "ProviderOrders");

            migrationBuilder.DropIndex(
                name: "IX_ProviderOrders_TurnId",
                table: "ProviderOrders");

            migrationBuilder.DropColumn(
                name: "TurnId",
                table: "ProviderOrders");

            migrationBuilder.CreateTable(
                name: "ProviderOrderTurn",
                columns: table => new
                {
                    ProviderOrdersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TurnsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderOrderTurn", x => new { x.ProviderOrdersId, x.TurnsId });
                    table.ForeignKey(
                        name: "FK_ProviderOrderTurn_ProviderOrders_ProviderOrdersId",
                        column: x => x.ProviderOrdersId,
                        principalTable: "ProviderOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProviderOrderTurn_Turns_TurnsId",
                        column: x => x.TurnsId,
                        principalTable: "Turns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProviderOrderTurn_TurnsId",
                table: "ProviderOrderTurn",
                column: "TurnsId");
        }
    }
}
