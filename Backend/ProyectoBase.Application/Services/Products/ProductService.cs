using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using ProyectoBase.Application.Abstractions;
using ProyectoBase.Application.DTOs;
using ProyectoBase.Domain.Entities;
using ProyectoBase.Domain.Exceptions;

namespace ProyectoBase.Application.Services.Products;

/// <summary>
/// Provides application level operations to manage products.
/// </summary>
public class ProductService : IProductService
{
    private const string AllProductsCacheKey = "products:all";

    private static readonly JsonSerializerOptions CacheSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductService"/> class.
    /// </summary>
    /// <param name="productRepository">The repository used to persist products.</param>
    /// <param name="mapper">The mapper used to project entities into DTOs.</param>
    /// <param name="cache">The distributed cache used to store product collections.</param>
    public ProductService(IProductRepository productRepository, IMapper mapper, IDistributedCache cache)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <inheritdoc />
    public async Task<ProductResponseDto> CreateAsync(ProductCreateDto product, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(product);

        var entity = _mapper.Map<Product>(product);

        await _productRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        await _cache.RemoveAsync(AllProductsCacheKey, cancellationToken).ConfigureAwait(false);

        return _mapper.Map<ProductResponseDto>(entity);
    }

    /// <inheritdoc />
    public async Task<ProductResponseDto> UpdateAsync(ProductUpdateDto product, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(product);

        var entity = await _productRepository.GetByIdAsync(product.Id, cancellationToken).ConfigureAwait(false);

        if (entity is null)
        {
            throw new NotFoundException($"The product with id '{product.Id}' was not found.");
        }

        _mapper.Map(product, entity);

        await _productRepository.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
        await _cache.RemoveAsync(AllProductsCacheKey, cancellationToken).ConfigureAwait(false);

        return _mapper.Map<ProductResponseDto>(entity);
    }

    /// <inheritdoc />
    public async Task<ProductResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);

        return product is null ? null : _mapper.Map<ProductResponseDto>(product);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ProductResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
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

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _productRepository.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        await _cache.RemoveAsync(AllProductsCacheKey, cancellationToken).ConfigureAwait(false);
    }
}
