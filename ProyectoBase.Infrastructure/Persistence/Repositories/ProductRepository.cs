using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProyectoBase.Application.Abstractions;
using ProyectoBase.Domain.Entities;

namespace ProyectoBase.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Provides Entity Framework Core based persistence operations for <see cref="Product"/> entities.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductRepository"/> class.
        /// </summary>
        /// <param name="context">The database context used to interact with the persistence store.</param>
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(product => product.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var products = await _context.Products
                .AsNoTracking()
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return products.AsReadOnly();
        }

        /// <inheritdoc />
        public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            await _context.Products.AddAsync(product, cancellationToken).ConfigureAwait(false);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken)
                .ConfigureAwait(false);

            if (product is null)
            {
                return;
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
