using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using ProyectoBase.Api.Application.Abstractions;
using ProyectoBase.Api.Domain.Entities;

namespace ProyectoBase.Api.Infrastructure.Persistence.Repositories;

/// <summary>
/// Decora una instancia de <see cref="IProductRepository"/> con políticas de resiliencia proporcionadas por Polly.
/// </summary>
public class ResilientProductRepository : IProductRepository
{
    private readonly IProductRepository _innerRepository;
    private readonly IAsyncPolicy _writePolicy;
    private readonly IAsyncPolicy _getPolicy;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="ResilientProductRepository"/>.
    /// </summary>
    /// <param name="innerRepository">El repositorio que se decorará.</param>
    /// <param name="writePolicy">La política que se aplicará a las operaciones de escritura.</param>
    /// <param name="getPolicy">La política que se aplicará a las operaciones de lectura.</param>
    public ResilientProductRepository(
        IProductRepository innerRepository,
        IAsyncPolicy writePolicy,
        IAsyncPolicy getPolicy)
    {
        _innerRepository = innerRepository ?? throw new ArgumentNullException(nameof(innerRepository));
        _writePolicy = writePolicy ?? throw new ArgumentNullException(nameof(writePolicy));
        _getPolicy = getPolicy ?? throw new ArgumentNullException(nameof(getPolicy));
    }

    /// <inheritdoc />
    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Product? product = null;

        await _getPolicy.ExecuteAsync(
            async (context, token) =>
            {
                product = await _innerRepository.GetByIdAsync(id, token).ConfigureAwait(false);
            },
            new Context(nameof(GetByIdAsync)),
            cancellationToken).ConfigureAwait(false);

        return product;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Product>? products = null;

        await _getPolicy.ExecuteAsync(
            async (context, token) =>
            {
                products = await _innerRepository.GetAllAsync(token).ConfigureAwait(false);
            },
            new Context(nameof(GetAllAsync)),
            cancellationToken).ConfigureAwait(false);

        return products ?? Array.Empty<Product>();
    }

    /// <inheritdoc />
    public Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(product);

        return _writePolicy.ExecuteAsync(
            (context, token) => _innerRepository.AddAsync(product, token),
            new Context(nameof(AddAsync)),
            cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(product);

        return _writePolicy.ExecuteAsync(
            (context, token) => _innerRepository.UpdateAsync(product, token),
            new Context(nameof(UpdateAsync)),
            cancellationToken);
    }

    /// <inheritdoc />
    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _writePolicy.ExecuteAsync(
            (context, token) => _innerRepository.DeleteAsync(id, token),
            new Context(nameof(DeleteAsync)),
            cancellationToken);
    }
}
