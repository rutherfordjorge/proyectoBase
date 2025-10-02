using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProyectoBase.Domain.Entities;

namespace ProyectoBase.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Defines the entity framework mapping configuration for <see cref="Product"/> entities.
    /// </summary>
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        /// <summary>
        /// Configures the entity type builder with the mapping rules for the <see cref="Product"/> entity.
        /// </summary>
        /// <param name="builder">The builder that is used to configure the entity type.</param>
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
