using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockSale.App.Migrations
{
    /// <inheritdoc />
    public partial class NClientAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NClient",
                table: "Clients",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NClient",
                table: "Clients");
        }
    }
}
