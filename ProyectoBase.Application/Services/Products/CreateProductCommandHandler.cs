using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProyectoBase.Application.Abstractions;
using ProyectoBase.Application.DTOs;

namespace ProyectoBase.Application.Services.Products
{
    /// <summary>
    /// Handles <see cref="CreateProductCommand"/> requests by delegating to the product service.
    /// </summary>
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductResponseDto>
    {
        private readonly IProductService _productService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateProductCommandHandler"/> class.
        /// </summary>
        /// <param name="productService">The product service used to perform the operation.</param>
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
