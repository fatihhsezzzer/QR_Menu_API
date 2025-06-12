using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mazina_Backend.Migrations
{
    /// <inheritdoc />
    public partial class CategoySize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Size",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "Categories");
        }
    }
}
