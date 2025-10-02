using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProyectoBase.Domain.Entities;

namespace ProyectoBase.Application.Abstractions
{
    /// <summary>
    /// Provides data access operations for <see cref="Product"/> entities.
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        /// <param name="id">The identifier of the product to retrieve.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The matching <see cref="Product"/> instance, if it exists.</returns>
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all products available in the data store.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A read-only collection containing the available products.</returns>
        Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Persists a new product in the data store.
        /// </summary>
        /// <param name="product">The product to persist.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddAsync(Product product, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing product in the data store.
        /// </summary>
        /// <param name="product">The product instance containing the updates.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpdateAsync(Product product, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a product from the data store.
        /// </summary>
        /// <param name="id">The identifier of the product to delete.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
