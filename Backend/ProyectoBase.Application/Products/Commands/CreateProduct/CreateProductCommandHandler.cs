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
using ProyectoBase.Api.Domain.Entities;
using ProyectoBase.Api.Domain.Exceptions;

namespace ProyectoBase.Api.Application.Products.Commands.CreateProduct;

/// <summary>
/// Controla la creación de nuevos productos mediante el patrón mediador.
/// </summary>
public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductResponseDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="CreateProductCommandHandler"/>.
    /// </summary>
    public CreateProductCommandHandler(
        IProductRepository productRepository,
        IMapper mapper,
        IDistributedCache cache,
        ILogger<CreateProductCommandHandler> logger)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<ProductResponseDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Product);

        try
        {
            var entity = _mapper.Map<Product>(request.Product);

            await _productRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            await _cache.RemoveAsync(ProductCacheSettings.AllProductsCacheKey, cancellationToken).ConfigureAwait(false);

            return _mapper.Map<ProductResponseDto>(entity);
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Ocurrió un error inesperado al crear el producto {ProductName}.", request.Product.Name);
            throw new ProductServiceException("No fue posible crear el producto debido a un error inesperado.", exception);
        }
    }
}
