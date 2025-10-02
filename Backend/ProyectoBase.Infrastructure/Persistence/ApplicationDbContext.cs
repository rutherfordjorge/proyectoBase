using Microsoft.EntityFrameworkCore;
using ProyectoBase.Domain.Entities;
using ProyectoBase.Infrastructure.Persistence.Configurations;

namespace ProyectoBase.Infrastructure.Persistence
{
    /// <summary>
    /// Representa el contexto de base de datos de Entity Framework Core encargado de administrar
    /// la capa de persistencia de la aplicación.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ApplicationDbContext"/>.
        /// </summary>
        /// <param name="options">Las opciones que utilizará el <see cref="DbContext"/>.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Obtiene o establece el conjunto de entidades <see cref="Product"/> administradas por el contexto.
        /// </summary>
        public DbSet<Product> Products { get; set; } = null!;

        /// <summary>
        /// Configura el modelo que vincula las entidades de dominio con el esquema de la base de datos.
        /// </summary>
        /// <param name="modelBuilder">El generador utilizado para construir el modelo del contexto.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductConfiguration).Assembly);
        }
    }
}
