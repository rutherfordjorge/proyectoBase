using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using ProyectoBase.Api.Application.Abstractions;
using ProyectoBase.Api.Application.Exceptions;
using ProyectoBase.Api.Application.Products.Common;
using ProyectoBase.Api.Domain.Exceptions;

namespace ProyectoBase.Api.Application.Products.Commands.DeleteProduct;

/// <summary>
/// Atiende la eliminación de productos.
/// </summary>
public sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IDistributedCache _cache;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="DeleteProductCommandHandler"/>.
    /// </summary>
    public DeleteProductCommandHandler(
        IProductRepository productRepository,
        IDistributedCache cache,
        ILogger<DeleteProductCommandHandler> logger)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            await _productRepository.DeleteAsync(request.Id, cancellationToken).ConfigureAwait(false);
            await _cache.RemoveAsync(ProductCacheSettings.AllProductsCacheKey, cancellationToken).ConfigureAwait(false);
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Ocurrió un error inesperado al eliminar el producto {ProductId}.", request.Id);
            throw new ProductServiceException("No fue posible eliminar el producto debido a un error inesperado.", exception);
        }
    }
}
