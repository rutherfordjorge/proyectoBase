using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using ProyectoBase.Api.Application.Abstractions;
using ProyectoBase.Api.Application.DTOs;
using ProyectoBase.Api.Application.Exceptions;
using ProyectoBase.Api.Application.Products.Common;
using ProyectoBase.Api.Domain.Exceptions;

namespace ProyectoBase.Api.Application.Products.Commands.UpdateProduct;

/// <summary>
/// Gestiona la actualización de productos existentes.
/// </summary>
public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductResponseDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="UpdateProductCommandHandler"/>.
    /// </summary>
    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        IMapper mapper,
        IDistributedCache cache,
        ILogger<UpdateProductCommandHandler> logger)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<ProductResponseDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Product);

        try
        {
            var entity = await _productRepository
                .GetByIdAsync(request.Product.Id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null)
            {
                throw new NotFoundException($"No se encontró el producto con identificador '{request.Product.Id}'.");
            }

            _mapper.Map(request.Product, entity);

            await _productRepository.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
            await _cache.RemoveAsync(ProductCacheSettings.AllProductsCacheKey, cancellationToken).ConfigureAwait(false);

            return _mapper.Map<ProductResponseDto>(entity);
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Ocurrió un error inesperado al actualizar el producto {ProductId}.", request.Product.Id);
            throw new ProductServiceException("No fue posible actualizar el producto debido a un error inesperado.", exception);
        }
    }
}
