using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProyectoBase.Api.Application.Abstractions;
using ProyectoBase.Api.Application.DTOs;
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
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="ProductsController"/>.
    /// </summary>
    /// <param name="productService">El servicio responsable de las operaciones de productos.</param>
    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
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
        try
        {
            var products = await _productService.GetAllAsync(cancellationToken).ConfigureAwait(false);

            return Ok(products);
        }
        catch (DomainException domainException)
        {
            _logger.LogWarning(domainException, "Se produjo un error de dominio al obtener el listado de productos.");
            return CreateErrorResponse(StatusCodes.Status400BadRequest, "Solicitud inválida", domainException.Message);
        }
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
        try
        {
            var product = await _productService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);

            if (product is null)
            {
                const string message = "El producto solicitado no fue encontrado.";
                _logger.LogWarning("No se encontró el producto con identificador {ProductId}.", id);
                return CreateErrorResponse(StatusCodes.Status404NotFound, "Recurso no encontrado", message);
            }

            return Ok(product);
        }
        catch (NotFoundException notFoundException)
        {
            _logger.LogWarning(notFoundException, "No se encontró el producto con identificador {ProductId}.", id);
            return CreateErrorResponse(StatusCodes.Status404NotFound, "Recurso no encontrado", notFoundException.Message);
        }
        catch (DomainException domainException)
        {
            _logger.LogWarning(domainException, "Se produjo un error de dominio al obtener el producto {ProductId}.", id);
            return CreateErrorResponse(StatusCodes.Status400BadRequest, "Solicitud inválida", domainException.Message);
        }
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
        try
        {
            var createdProduct = await _productService.CreateAsync(product, cancellationToken).ConfigureAwait(false);
            var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";

            return CreatedAtAction(nameof(GetProductByIdAsync), new { id = createdProduct.Id, version }, createdProduct);
        }
        catch (Domain.Exceptions.ValidationException validationException)
        {
            _logger.LogWarning(validationException, "Los datos del nuevo producto no superaron la validación de dominio.");
            return CreateErrorResponse(StatusCodes.Status400BadRequest, "Solicitud inválida", validationException.Message);
        }
        catch (ValidationException validationException)
        {
            var details = GetFluentValidationDetails(validationException);
            _logger.LogWarning(validationException, "Los datos del nuevo producto no superaron la validación del modelo.");
            return CreateErrorResponse(StatusCodes.Status400BadRequest, "Solicitud inválida", details);
        }
        catch (DomainException domainException)
        {
            _logger.LogWarning(domainException, "Se produjo un error de dominio al crear un producto.");
            return CreateErrorResponse(StatusCodes.Status400BadRequest, "Solicitud inválida", domainException.Message);
        }
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
            return BadRequest("El identificador proporcionado no coincide con el valor de la ruta.");
        }

        product.Id = id;

        try
        {
            var updatedProduct = await _productService.UpdateAsync(product, cancellationToken).ConfigureAwait(false);

            return Ok(updatedProduct);
        }
        catch (NotFoundException notFoundException)
        {
            _logger.LogWarning(notFoundException, "No se encontró el producto con identificador {ProductId} para actualizar.", id);
            return CreateErrorResponse(StatusCodes.Status404NotFound, "Recurso no encontrado", notFoundException.Message);
        }
        catch (Domain.Exceptions.ValidationException validationException)
        {
            _logger.LogWarning(validationException, "Los datos del producto {ProductId} no superaron la validación de dominio.", id);
            return CreateErrorResponse(StatusCodes.Status400BadRequest, "Solicitud inválida", validationException.Message);
        }
        catch (ValidationException validationException)
        {
            var details = GetFluentValidationDetails(validationException);
            _logger.LogWarning(validationException, "Los datos del producto {ProductId} no superaron la validación del modelo.", id);
            return CreateErrorResponse(StatusCodes.Status400BadRequest, "Solicitud inválida", details);
        }
        catch (DomainException domainException)
        {
            _logger.LogWarning(domainException, "Se produjo un error de dominio al actualizar el producto {ProductId}.", id);
            return CreateErrorResponse(StatusCodes.Status400BadRequest, "Solicitud inválida", domainException.Message);
        }
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
        try
        {
            var existingProduct = await _productService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);

            if (existingProduct is null)
            {
                const string message = "El producto solicitado no fue encontrado.";
                _logger.LogWarning("No se encontró el producto con identificador {ProductId} para eliminar.", id);
                return CreateErrorResponse(StatusCodes.Status404NotFound, "Recurso no encontrado", message);
            }

            await _productService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }
        catch (NotFoundException notFoundException)
        {
            _logger.LogWarning(notFoundException, "No se encontró el producto con identificador {ProductId} para eliminar.", id);
            return CreateErrorResponse(StatusCodes.Status404NotFound, "Recurso no encontrado", notFoundException.Message);
        }
        catch (DomainException domainException)
        {
            _logger.LogWarning(domainException, "Se produjo un error de dominio al eliminar el producto {ProductId}.", id);
            return CreateErrorResponse(StatusCodes.Status400BadRequest, "Solicitud inválida", domainException.Message);
        }
    }

    private ObjectResult CreateErrorResponse(int statusCode, string error, object? details)
    {
        var traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

        var payload = new
        {
            traceId,
            status = statusCode,
            error,
            details,
        };

        return StatusCode(statusCode, payload);
    }

    private static IReadOnlyCollection<string> GetFluentValidationDetails(ValidationException exception)
    {
        return exception.Errors
            .Select(error => error.ErrorMessage)
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .Distinct(StringComparer.Ordinal)
            .ToArray();
    }
}
