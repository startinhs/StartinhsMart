using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StartinhsMart.CoreService.Migrations
{
    /// <inheritdoc />
    public partial class Added_Pet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppFileDescriptors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    MimeType = table.Column<string>(type: "text", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppFileDescriptors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppPets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ImageId = table.Column<Guid>(type: "uuid", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Breed = table.Column<string>(type: "text", nullable: true),
                    Age = table.Column<float>(type: "real", nullable: true),
                    Gender = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: true),
                    Weight = table.Column<float>(type: "real", nullable: true),
                    HealthStatus = table.Column<string>(type: "text", nullable: true),
                    Vaccinations = table.Column<int>(type: "integer", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: true),
                    IsBooth = table.Column<bool>(type: "boolean", nullable: false),
                    IsStock = table.Column<bool>(type: "boolean", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppPets", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppFileDescriptors");

            migrationBuilder.DropTable(
                name: "AppPets");
        }
    }
}
