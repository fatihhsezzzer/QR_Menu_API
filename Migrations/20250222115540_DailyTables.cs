using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mazina_Backend.Migrations
{
    /// <inheritdoc />
    public partial class DailyTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyProductsInserted",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyProductsInserted", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "DailyProductsUpdated",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    OldPrice = table.Column<float>(type: "real", nullable: false),
                    NewPrice = table.Column<float>(type: "real", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyProductsUpdated", x => x.ProductId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyProductsInserted");

            migrationBuilder.DropTable(
                name: "DailyProductsUpdated");
        }
    }
}
