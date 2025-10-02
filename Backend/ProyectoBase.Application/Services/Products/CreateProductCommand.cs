using MediatR;
using ProyectoBase.Api.Application.DTOs;

namespace ProyectoBase.Api.Application.Services.Products
{
    /// <summary>
    /// Comando utilizado para solicitar la creación de un nuevo producto.
    /// </summary>
    public class CreateProductCommand : IRequest<ProductResponseDto>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="CreateProductCommand"/>.
        /// </summary>
        /// <param name="product">La información del producto asociada al comando.</param>
        public CreateProductCommand(ProductCreateDto product)
        {
            Product = product;
        }

        /// <summary>
        /// Obtiene la información del producto proporcionada en el comando.
        /// </summary>
        public ProductCreateDto Product { get; }
    }
}
