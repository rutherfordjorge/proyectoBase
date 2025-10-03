using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ProyectoBase.Api.Application.Abstractions;
using ProyectoBase.Api.Application.DTOs;
using ProyectoBase.Api.Application.Exceptions;
using ProyectoBase.Api.Domain.Exceptions;

namespace ProyectoBase.Api.Application.Products.Queries.GetProductById;

/// <summary>
/// Gestiona la obtención de un producto específico.
/// </summary>
public sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductResponseDto?>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="GetProductByIdQueryHandler"/>.
    /// </summary>
    public GetProductByIdQueryHandler(
        IProductRepository productRepository,
        IMapper mapper,
        ILogger<GetProductByIdQueryHandler> logger)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<ProductResponseDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);

            return product is null ? null : _mapper.Map<ProductResponseDto>(product);
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Ocurrió un error inesperado al obtener el producto {ProductId}.", request.Id);
            throw new ProductServiceException("No fue posible obtener el producto debido a un error inesperado.", exception);
        }
    }
}
