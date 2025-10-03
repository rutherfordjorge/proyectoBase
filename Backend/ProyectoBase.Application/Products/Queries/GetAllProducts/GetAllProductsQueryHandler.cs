using System;
using System.Collections.Generic;
using System.Text.Json;
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

namespace ProyectoBase.Api.Application.Products.Queries.GetAllProducts;

/// <summary>
/// Gestiona la obtención del listado completo de productos con soporte de caché distribuida.
/// </summary>
public sealed class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IReadOnlyCollection<ProductResponseDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;
    private readonly ILogger<GetAllProductsQueryHandler> _logger;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="GetAllProductsQueryHandler"/>.
    /// </summary>
    public GetAllProductsQueryHandler(
        IProductRepository productRepository,
        IMapper mapper,
        IDistributedCache cache,
        ILogger<GetAllProductsQueryHandler> logger)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ProductResponseDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cachedProducts = await _cache.GetAsync(ProductCacheSettings.AllProductsCacheKey, cancellationToken).ConfigureAwait(false);

            if (cachedProducts is not null)
            {
                var cachedCollection = JsonSerializer.Deserialize<List<ProductResponseDto>>(cachedProducts, ProductCacheSettings.SerializerOptions);

                if (cachedCollection is not null)
                {
                    return cachedCollection.Count == 0
                        ? Array.Empty<ProductResponseDto>()
                        : cachedCollection.AsReadOnly();
                }
            }

            var products = await _productRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
            var mapped = _mapper.Map<List<ProductResponseDto>>(products);

            var cacheEntryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            };

            await _cache.SetAsync(
                ProductCacheSettings.AllProductsCacheKey,
                JsonSerializer.SerializeToUtf8Bytes(mapped, ProductCacheSettings.SerializerOptions),
                cacheEntryOptions,
                cancellationToken).ConfigureAwait(false);

            return mapped.Count == 0 ? Array.Empty<ProductResponseDto>() : mapped.AsReadOnly();
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Ocurrió un error inesperado al obtener la lista de productos.");
            throw new ProductServiceException("No fue posible obtener la lista de productos debido a un error inesperado.", exception);
        }
    }
}
