using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mazina_Backend.Migrations
{
    /// <inheritdoc />
    public partial class DailyTablesDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "DailyProductsUpdated",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "DailyProductsInserted",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "DailyProductsUpdated");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "DailyProductsInserted");
        }
    }
}
