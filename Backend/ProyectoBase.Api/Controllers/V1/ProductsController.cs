using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProyectoBase.Api.Application.DTOs;
using ProyectoBase.Api.Application.Products.Commands.CreateProduct;
using ProyectoBase.Api.Application.Products.Commands.DeleteProduct;
using ProyectoBase.Api.Application.Products.Commands.UpdateProduct;
using ProyectoBase.Api.Application.Products.Queries.GetAllProducts;
using ProyectoBase.Api.Application.Products.Queries.GetProductById;
using ProyectoBase.Api.Domain.Exceptions;

namespace ProyectoBase.Api.Api.Controllers.V1;

/// <summary>
/// Expone endpoints para administrar recursos de productos.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="ProductsController"/>.
    /// </summary>
    /// <param name="mediator">El mediador encargado de coordinar las operaciones de productos.</param>
    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Recupera la lista completa de productos disponibles.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelar la operación asincrónica.</param>
    /// <returns>Una colección con los productos solicitados.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ProductResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<ProductResponseDto>>> GetProductsAsync(CancellationToken cancellationToken)
    {
        var products = await _mediator.Send(new GetAllProductsQuery(), cancellationToken).ConfigureAwait(false);

        return Ok(products);
    }

    /// <summary>
    /// Recupera un producto específico por su identificador único.
    /// </summary>
    /// <param name="id">El identificador del producto que se desea obtener.</param>
    /// <param name="cancellationToken">Token para cancelar la operación asincrónica.</param>
    /// <returns>El producto que coincide con el identificador proporcionado.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductResponseDto>> GetProductByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id), cancellationToken).ConfigureAwait(false);

        if (product is null)
        {
            _logger.LogWarning("No se encontró el producto con identificador {ProductId}.", id);
            throw new NotFoundException("El producto solicitado no fue encontrado.");
        }

        return Ok(product);
    }

    /// <summary>
    /// Crea un nuevo producto con la información proporcionada.
    /// </summary>
    /// <param name="product">La información del producto.</param>
    /// <param name="cancellationToken">Token para cancelar la operación asincrónica.</param>
    /// <returns>La representación del producto creado.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductResponseDto>> CreateProductAsync([FromBody] ProductCreateDto product, CancellationToken cancellationToken)
    {
        var createdProduct = await _mediator.Send(new CreateProductCommand(product), cancellationToken).ConfigureAwait(false);
        var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";

        return CreatedAtAction(nameof(GetProductByIdAsync), new { id = createdProduct.Id, version }, createdProduct);
    }

    /// <summary>
    /// Actualiza el producto que coincide con el identificador proporcionado.
    /// </summary>
    /// <param name="id">El identificador del producto que se desea actualizar.</param>
    /// <param name="product">Los datos del producto que se aplicarán.</param>
    /// <param name="cancellationToken">Token para cancelar la operación asincrónica.</param>
    /// <returns>La representación del producto actualizado.</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponseDto>> UpdateProductAsync(Guid id, [FromBody] ProductUpdateDto product, CancellationToken cancellationToken)
    {
        if (product.Id != Guid.Empty && product.Id != id)
        {
            throw new ValidationException("El identificador proporcionado no coincide con el valor de la ruta.");
        }

        product.Id = id;

        var updatedProduct = await _mediator.Send(new UpdateProductCommand(product), cancellationToken).ConfigureAwait(false);

        return Ok(updatedProduct);
    }

    /// <summary>
    /// Elimina el producto que coincide con el identificador proporcionado.
    /// </summary>
    /// <param name="id">El identificador del producto que se eliminará.</param>
    /// <param name="cancellationToken">Token para cancelar la operación asincrónica.</param>
    /// <returns>Una respuesta que indica el resultado de la eliminación.</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteProductAsync(Guid id, CancellationToken cancellationToken)
    {
        var existingProduct = await _mediator.Send(new GetProductByIdQuery(id), cancellationToken).ConfigureAwait(false);

        if (existingProduct is null)
        {
            _logger.LogWarning("No se encontró el producto con identificador {ProductId} para eliminar.", id);
            throw new NotFoundException("El producto solicitado no fue encontrado.");
        }

        await _mediator.Send(new DeleteProductCommand(id), cancellationToken).ConfigureAwait(false);

        return NoContent();
    }
}
