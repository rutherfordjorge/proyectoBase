using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProyectoBase.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Defines the initial database schema for the application including the Products table.
    /// </summary>
    public partial class InitialCreate : Migration
    {
        /// <summary>
        /// Builds the database objects required for the initial release of the application.
        /// </summary>
        /// <param name="migrationBuilder">The builder used to construct the database schema.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });
        }

        /// <summary>
        /// Removes the database objects created in the <see cref="Up(MigrationBuilder)"/> method.
        /// </summary>
        /// <param name="migrationBuilder">The builder used to drop the database schema.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
