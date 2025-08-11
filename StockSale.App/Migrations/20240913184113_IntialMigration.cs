using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockSale.App.Migrations
{
    /// <inheritdoc />
    public partial class IntialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dni = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Condition_Iva = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discount = table.Column<int>(type: "int", nullable: true),
                    Debt = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fiados",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Import = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fiados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Providers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Providers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Turns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Initial_Cash = table.Column<float>(type: "real", nullable: false),
                    Closing_Cash = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnitsM",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitsM", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProviderOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Import = table.Column<float>(type: "real", nullable: false),
                    ProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderOrders_Providers_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Providers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClientTurn",
                columns: table => new
                {
                    ClientsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TurnsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientTurn", x => new { x.ClientsId, x.TurnsId });
                    table.ForeignKey(
                        name: "FK_ClientTurn_Clients_ClientsId",
                        column: x => x.ClientsId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientTurn_Turns_TurnsId",
                        column: x => x.TurnsId,
                        principalTable: "Turns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderBuys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Import = table.Column<float>(type: "real", nullable: false),
                    Discount = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TurnId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderBuys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderBuys_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderBuys_Turns_TurnId",
                        column: x => x.TurnId,
                        principalTable: "Turns",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Packaging = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    Stock_Limit = table.Column<int>(type: "int", nullable: false),
                    List_Price = table.Column<int>(type: "int", nullable: false),
                    Sell_Price = table.Column<int>(type: "int", nullable: false),
                    ProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnitMId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Providers_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Providers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Products_UnitsM_UnitMId",
                        column: x => x.UnitMId,
                        principalTable: "UnitsM",
                        principalColumn: "Id");
                });

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

            migrationBuilder.CreateTable(
                name: "Products_Price",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriceList = table.Column<float>(type: "real", nullable: false),
                    PriceSell = table.Column<float>(type: "real", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrderBuyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProviderOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products_Price", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Price_OrderBuys_OrderBuyId",
                        column: x => x.OrderBuyId,
                        principalTable: "OrderBuys",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Products_Price_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Products_Price_ProviderOrders_ProviderOrderId",
                        column: x => x.ProviderOrderId,
                        principalTable: "ProviderOrders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientTurn_TurnsId",
                table: "ClientTurn",
                column: "TurnsId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderBuys_ClientId",
                table: "OrderBuys",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderBuys_TurnId",
                table: "OrderBuys",
                column: "TurnId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProviderId",
                table: "Products",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UnitMId",
                table: "Products",
                column: "UnitMId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Price_OrderBuyId",
                table: "Products_Price",
                column: "OrderBuyId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Price_ProductId",
                table: "Products_Price",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Price_ProviderOrderId",
                table: "Products_Price",
                column: "ProviderOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderOrders_ProviderId",
                table: "ProviderOrders",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderOrderTurn_TurnsId",
                table: "ProviderOrderTurn",
                column: "TurnsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientTurn");

            migrationBuilder.DropTable(
                name: "Fiados");

            migrationBuilder.DropTable(
                name: "Products_Price");

            migrationBuilder.DropTable(
                name: "ProviderOrderTurn");

            migrationBuilder.DropTable(
                name: "OrderBuys");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "ProviderOrders");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Turns");

            migrationBuilder.DropTable(
                name: "UnitsM");

            migrationBuilder.DropTable(
                name: "Providers");
        }
    }
}
