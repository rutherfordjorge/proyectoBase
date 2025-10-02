using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProyectoBase.Api.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Define el esquema inicial de la base de datos para la aplicación, incluida la tabla de productos.
    /// </summary>
    public partial class InitialCreate : Migration
    {
        /// <summary>
        /// Crea los objetos de base de datos necesarios para la primera versión de la aplicación.
        /// </summary>
        /// <param name="migrationBuilder">El generador utilizado para construir el esquema de la base de datos.</param>
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
        /// Elimina los objetos de base de datos creados en el método <see cref="Up(MigrationBuilder)"/>.
        /// </summary>
        /// <param name="migrationBuilder">El generador utilizado para eliminar el esquema de la base de datos.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
