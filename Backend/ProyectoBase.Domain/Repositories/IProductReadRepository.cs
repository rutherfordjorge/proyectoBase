using System;
using System.Threading;
using System.Threading.Tasks;
using ProyectoBase.Domain.Entities;

namespace ProyectoBase.Domain.Repositories
{
    /// <summary>
    /// Provides read-only access to <see cref="Product"/> entities from a persistence store.
    /// </summary>
    public interface IProductReadRepository
    {
        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        /// <param name="id">The identifier of the product to retrieve.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The product that matches the provided identifier, or <c>null</c> if it does not exist.</returns>
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
