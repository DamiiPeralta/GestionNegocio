using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockSale.App.Migrations
{
    /// <inheritdoc />
    public partial class FiadoIsPayPropertyAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPay",
                table: "Fiados",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPay",
                table: "Fiados");
        }
    }
}
