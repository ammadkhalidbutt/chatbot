using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ColumnWiseSearch.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedDate", "Description", "IsAvailable", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Electronics", new DateTime(2023, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "High performance laptop", true, "Laptop", 1200.00m },
                    { 2, "Electronics", new DateTime(2023, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Latest model smartphone", true, "Smartphone", 800.00m },
                    { 3, "Audio", new DateTime(2022, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Noise cancelling headphones", false, "Headphones", 250.00m },
                    { 4, "Kitchen", new DateTime(2023, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Automatic coffee machine", true, "Coffee Maker", 150.00m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
