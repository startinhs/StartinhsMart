using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StartinhsMart.CoreService.Migrations
{
    /// <inheritdoc />
    public partial class ChangePetCategoryToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "AppPets");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "AppPets",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "AppPets");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "AppPets",
                type: "text",
                nullable: true);
        }
    }
}
