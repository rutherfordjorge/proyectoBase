using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using ProyectoBase.Application.Abstractions;
using ProyectoBase.Application.DTOs;
using ProyectoBase.Application.Exceptions;
using ProyectoBase.Domain.Entities;
using ProyectoBase.Domain.Exceptions;

namespace ProyectoBase.Application.Services.Products;

/// <summary>
/// Proporciona operaciones de nivel de aplicación para administrar productos.
/// </summary>
public class ProductService : IProductService
{
    private const string AllProductsCacheKey = "products:all";

    private static readonly JsonSerializerOptions CacheSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;
    private readonly ILogger<ProductService> _logger;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="ProductService"/>.
    /// </summary>
    /// <param name="productRepository">El repositorio utilizado para persistir productos.</param>
    /// <param name="mapper">El mapeador utilizado para proyectar entidades en DTO.</param>
    /// <param name="cache">La caché distribuida donde se almacenan las colecciones de productos.</param>
    public ProductService(IProductRepository productRepository, IMapper mapper, IDistributedCache cache, ILogger<ProductService> logger)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<ProductResponseDto> CreateAsync(ProductCreateDto product, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(product);

        try
        {
            var entity = _mapper.Map<Product>(product);

            await _productRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            await _cache.RemoveAsync(AllProductsCacheKey, cancellationToken).ConfigureAwait(false);

            return _mapper.Map<ProductResponseDto>(entity);
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Ocurrió un error inesperado al crear el producto {ProductName}.", product.Name);
            throw new ProductServiceException("No fue posible crear el producto debido a un error inesperado.", exception);
        }
    }

    /// <inheritdoc />
    public async Task<ProductResponseDto> UpdateAsync(ProductUpdateDto product, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(product);

        try
        {
            var entity = await _productRepository.GetByIdAsync(product.Id, cancellationToken).ConfigureAwait(false);

            if (entity is null)
            {
                throw new NotFoundException($"No se encontró el producto con identificador '{product.Id}'.");
            }

            _mapper.Map(product, entity);

            await _productRepository.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
            await _cache.RemoveAsync(AllProductsCacheKey, cancellationToken).ConfigureAwait(false);

            return _mapper.Map<ProductResponseDto>(entity);
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Ocurrió un error inesperado al actualizar el producto {ProductId}.", product.Id);
            throw new ProductServiceException("No fue posible actualizar el producto debido a un error inesperado.", exception);
        }
    }

    /// <inheritdoc />
    public async Task<ProductResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);

            return product is null ? null : _mapper.Map<ProductResponseDto>(product);
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Ocurrió un error inesperado al obtener el producto {ProductId}.", id);
            throw new ProductServiceException("No fue posible obtener el producto debido a un error inesperado.", exception);
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ProductResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var cachedProducts = await _cache.GetAsync(AllProductsCacheKey, cancellationToken).ConfigureAwait(false);

            if (cachedProducts is not null)
            {
                var cachedCollection = JsonSerializer.Deserialize<List<ProductResponseDto>>(cachedProducts, CacheSerializerOptions);

                if (cachedCollection is not null && cachedCollection.Count > 0)
                {
                    return cachedCollection.AsReadOnly();
                }

                if (cachedCollection is not null)
                {
                    return Array.Empty<ProductResponseDto>();
                }
            }

            var products = await _productRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);

            var mapped = _mapper.Map<List<ProductResponseDto>>(products);
            var cacheEntryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            };

            await _cache.SetAsync(
                AllProductsCacheKey,
                JsonSerializer.SerializeToUtf8Bytes(mapped, CacheSerializerOptions),
                cacheEntryOptions,
                cancellationToken).ConfigureAwait(false);

            return mapped.Count == 0
                ? Array.Empty<ProductResponseDto>()
                : mapped.AsReadOnly();
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

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _productRepository.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
            await _cache.RemoveAsync(AllProductsCacheKey, cancellationToken).ConfigureAwait(false);
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Ocurrió un error inesperado al eliminar el producto {ProductId}.", id);
            throw new ProductServiceException("No fue posible eliminar el producto debido a un error inesperado.", exception);
        }
    }
}
