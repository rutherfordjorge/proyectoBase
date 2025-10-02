using Microsoft.EntityFrameworkCore;
using ProyectoBase.Domain.Entities;
using ProyectoBase.Infrastructure.Persistence.Configurations;

namespace ProyectoBase.Infrastructure.Persistence
{
    /// <summary>
    /// Represents the Entity Framework Core database context responsible for managing
    /// the persistence layer of the application.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by the <see cref="DbContext"/>.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the set of <see cref="Product"/> entities tracked by the context.
        /// </summary>
        public DbSet<Product> Products { get; set; } = null!;

        /// <summary>
        /// Configures the model that maps the domain entities to the database schema.
        /// </summary>
        /// <param name="modelBuilder">The builder used to construct the model for the context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductConfiguration).Assembly);
        }
    }
}
