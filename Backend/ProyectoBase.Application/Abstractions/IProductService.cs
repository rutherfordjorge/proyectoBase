using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProyectoBase.Application.DTOs;

namespace ProyectoBase.Application.Abstractions
{
    /// <summary>
    /// Defines the operations available to manage products within the application layer.
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Creates a new product from the provided information.
        /// </summary>
        /// <param name="product">The data describing the product to create.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The representation of the created product.</returns>
        Task<ProductResponseDto> CreateAsync(ProductCreateDto product, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing product using the supplied information.
        /// </summary>
        /// <param name="product">The data describing the product to update.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The representation of the updated product.</returns>
        Task<ProductResponseDto> UpdateAsync(ProductUpdateDto product, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        /// <param name="id">The identifier of the product to retrieve.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The requested product representation, if it exists.</returns>
        Task<ProductResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all products available in the application context.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection containing the available product representations.</returns>
        Task<IReadOnlyCollection<ProductResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the product that matches the provided identifier.
        /// </summary>
        /// <param name="id">The identifier of the product to remove.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
