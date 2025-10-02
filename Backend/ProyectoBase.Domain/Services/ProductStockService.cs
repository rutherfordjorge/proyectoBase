using System;
using System.Threading;
using System.Threading.Tasks;
using ProyectoBase.Domain.Entities;
using ProyectoBase.Domain.Exceptions;
using ProyectoBase.Domain.Repositories;

namespace ProyectoBase.Domain.Services
{
    /// <summary>
    /// Proporciona operaciones de dominio relacionadas con la disponibilidad de inventario de productos.
    /// </summary>
    public class ProductStockService
    {
        private readonly IProductReadRepository _productRepository;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ProductStockService"/>.
        /// </summary>
        /// <param name="productRepository">El repositorio utilizado para consultar productos.</param>
        public ProductStockService(IProductReadRepository productRepository)
        {
            ArgumentNullException.ThrowIfNull(productRepository);

            _productRepository = productRepository;
        }

        /// <summary>
        /// Determina si un producto cuenta con inventario suficiente para cubrir la cantidad solicitada.
        /// </summary>
        /// <param name="productId">El identificador del producto a evaluar.</param>
        /// <param name="quantity">La cantidad que se desea verificar.</param>
        /// <param name="cancellationToken">Token para cancelar la operación asincrónica.</param>
        /// <returns><c>true</c> cuando la cantidad solicitada está disponible; de lo contrario, <c>false</c>.</returns>
        /// <exception cref="NotFoundException">Se produce cuando no se encuentra el producto.</exception>
        /// <exception cref="ValidationException">Se produce cuando la cantidad solicitada no es mayor que cero.</exception>
        public async Task<bool> IsStockAvailableAsync(Guid productId, int quantity, CancellationToken cancellationToken)
        {
            if (quantity <= 0)
            {
                throw new ValidationException("La cantidad a verificar debe ser mayor que cero.");
            }

            var product = await _productRepository.GetByIdAsync(productId, cancellationToken).ConfigureAwait(false);

            if (product is null)
            {
                throw new NotFoundException($"No se encontró el producto '{productId}'.");
            }

            return product.Stock >= quantity;
        }
    }
}
