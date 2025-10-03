using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProyectoBase.Api.Domain.Entities;
using ProyectoBase.Api.Domain.ValueObjects;

namespace ProyectoBase.Api.Infrastructure.Persistence.Configurations
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
                .HasConversion(name => name.Value, value => ProductName.Create(value))
                .HasMaxLength(ProductName.MaxLength);

            builder.Property(product => product.Description)
                .HasConversion(description => description != null ? description.Value : null, value => ProductDescription.Create(value))
                .HasMaxLength(ProductDescription.MaxLength);

            builder.Property(product => product.Price)
                .HasConversion(price => price.Amount, value => Money.From(value))
                .HasColumnType("decimal(18,2)");

            builder.Property(product => product.Stock)
                .IsRequired()
                .HasConversion(stock => stock.Value, value => ProductStock.Create(value));
        }
    }
}
