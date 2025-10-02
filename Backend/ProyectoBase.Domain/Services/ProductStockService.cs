using System;
using System.Threading;
using System.Threading.Tasks;
using ProyectoBase.Domain.Entities;
using ProyectoBase.Domain.Exceptions;
using ProyectoBase.Domain.Repositories;

namespace ProyectoBase.Domain.Services
{
    /// <summary>
    /// Provides domain operations related to product stock availability.
    /// </summary>
    public class ProductStockService
    {
        private readonly IProductReadRepository _productRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductStockService"/> class.
        /// </summary>
        /// <param name="productRepository">The repository used to query products.</param>
        public ProductStockService(IProductReadRepository productRepository)
        {
            ArgumentNullException.ThrowIfNull(productRepository);

            _productRepository = productRepository;
        }

        /// <summary>
        /// Determines whether a product has enough stock to satisfy the requested quantity.
        /// </summary>
        /// <param name="productId">The identifier of the product to evaluate.</param>
        /// <param name="quantity">The quantity to verify.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns><c>true</c> when the requested quantity is available; otherwise, <c>false</c>.</returns>
        /// <exception cref="NotFoundException">Thrown when the product cannot be found.</exception>
        /// <exception cref="ValidationException">Thrown when the requested quantity is not greater than zero.</exception>
        public async Task<bool> IsStockAvailableAsync(Guid productId, int quantity, CancellationToken cancellationToken)
        {
            if (quantity <= 0)
            {
                throw new ValidationException("The quantity to check must be greater than zero.");
            }

            var product = await _productRepository.GetByIdAsync(productId, cancellationToken).ConfigureAwait(false);

            if (product is null)
            {
                throw new NotFoundException($"The product '{productId}' was not found.");
            }

            return product.Stock >= quantity;
        }
    }
}
