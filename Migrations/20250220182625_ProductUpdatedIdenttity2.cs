using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mazina_Backend.Migrations
{
    /// <inheritdoc />
    public partial class ProductUpdatedIdenttity2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ProductsUpdated",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ProductsUpdated");
        }
    }
}
