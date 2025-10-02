using MediatR;
using ProyectoBase.Application.DTOs;

namespace ProyectoBase.Application.Services.Products
{
    /// <summary>
    /// Command used to request the creation of a new product.
    /// </summary>
    public class CreateProductCommand : IRequest<ProductResponseDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateProductCommand"/> class.
        /// </summary>
        /// <param name="product">The product data associated with the command.</param>
        public CreateProductCommand(ProductCreateDto product)
        {
            Product = product;
        }

        /// <summary>
        /// Gets the product data provided with the command.
        /// </summary>
        public ProductCreateDto Product { get; }
    }
}
