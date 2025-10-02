using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
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
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductService"/> class.
    /// </summary>
    /// <param name="productRepository">The repository used to persist products.</param>
    /// <param name="mapper">The mapper used to project entities into DTOs.</param>
    public ProductService(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<ProductResponseDto> CreateAsync(ProductCreateDto product, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(product);

        var entity = _mapper.Map<Product>(product);

        await _productRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

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
        var products = await _productRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);

        var mapped = _mapper.Map<List<ProductResponseDto>>(products);

        return mapped.Count == 0
            ? Array.Empty<ProductResponseDto>()
            : mapped.AsReadOnly();
    }

    /// <inheritdoc />
    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _productRepository.DeleteAsync(id, cancellationToken);
    }
}
