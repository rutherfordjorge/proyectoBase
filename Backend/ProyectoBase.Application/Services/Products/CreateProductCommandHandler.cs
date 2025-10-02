using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProyectoBase.Api.Application.Abstractions;
using ProyectoBase.Api.Application.DTOs;

namespace ProyectoBase.Api.Application.Services.Products
{
    /// <summary>
    /// Atiende solicitudes de <see cref="CreateProductCommand"/> delegando en el servicio de productos.
    /// </summary>
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductResponseDto>
    {
        private readonly IProductService _productService;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="CreateProductCommandHandler"/>.
        /// </summary>
        /// <param name="productService">El servicio de productos que ejecuta la operación.</param>
        public CreateProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }

        /// <inheritdoc />
        public Task<ProductResponseDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            return _productService.CreateAsync(request.Product, cancellationToken);
        }
    }
}
