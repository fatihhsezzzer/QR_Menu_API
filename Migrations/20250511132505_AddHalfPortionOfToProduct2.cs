﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mazina_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddHalfPortionOfToProduct2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HalfPortionOf",
                table: "Products",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HalfPortionOf",
                table: "Products");
        }
    }
}
