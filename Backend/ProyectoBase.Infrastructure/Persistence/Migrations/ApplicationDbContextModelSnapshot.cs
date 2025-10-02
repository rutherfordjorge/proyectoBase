using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProyectoBase.Infrastructure.Persistence;

namespace ProyectoBase.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Representa la instantánea del modelo de Entity Framework Core para el contexto de base de datos de la aplicación.
    /// </summary>
    [DbContext(typeof(ApplicationDbContext))]
    internal class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        /// <summary>
        /// Construye el modelo que se utiliza para generar las migraciones de base de datos.
        /// </summary>
        /// <param name="modelBuilder">El generador del modelo empleado para configurar el mapeo de las entidades.</param>
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.9");

            modelBuilder.Entity("ProyectoBase.Domain.Entities.Product", builder =>
            {
                builder.Property<Guid>("Id")
                    .HasColumnType("uniqueidentifier");

                builder.Property<string>("Description")
                    .HasMaxLength(500)
                    .HasColumnType("nvarchar(500)");

                builder.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("nvarchar(200)");

                builder.Property<decimal>("Price")
                    .HasColumnType("decimal(18,2)");

                builder.Property<int>("Stock")
                    .HasColumnType("int");

                builder.HasKey("Id");

                builder.ToTable("Products");
            });
#pragma warning restore 612, 618
        }
    }
}
