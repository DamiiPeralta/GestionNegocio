using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockSale.App.Migrations
{
    /// <inheritdoc />
    public partial class NProductApplied : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NProduct",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NProduct",
                table: "Products");
        }
    }
}
