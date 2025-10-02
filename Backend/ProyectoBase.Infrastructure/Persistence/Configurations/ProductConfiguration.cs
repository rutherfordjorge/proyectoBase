using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProyectoBase.Domain.Entities;

namespace ProyectoBase.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Define la configuración de mapeo de Entity Framework para las entidades <see cref="Product"/>.
    /// </summary>
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        /// <summary>
        /// Configura el generador de tipos de entidad con las reglas de mapeo para la entidad <see cref="Product"/>.
        /// </summary>
        /// <param name="builder">El generador que se utiliza para configurar el tipo de entidad.</param>
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(product => product.Id);

            builder.Property(product => product.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(product => product.Description)
                .HasMaxLength(500);

            builder.Property(product => product.Price)
                .HasColumnType("decimal(18,2)");

            builder.Property(product => product.Stock)
                .IsRequired();
        }
    }
}
