using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoBase.Application.Abstractions;
using ProyectoBase.Application.DTOs;

namespace ProyectoBase.Api.Controllers.V1;

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

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="ProductsController"/>.
    /// </summary>
    /// <param name="productService">El servicio responsable de las operaciones de productos.</param>
    public ProductsController(IProductService productService)
    {
        _productService = productService;
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
        var products = await _productService.GetAllAsync(cancellationToken).ConfigureAwait(false);

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
        var product = await _productService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);

        if (product is null)
        {
            return NotFound();
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
        var createdProduct = await _productService.CreateAsync(product, cancellationToken).ConfigureAwait(false);
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
            return BadRequest("El identificador proporcionado no coincide con el valor de la ruta.");
        }

        product.Id = id;

        var updatedProduct = await _productService.UpdateAsync(product, cancellationToken).ConfigureAwait(false);

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
        var existingProduct = await _productService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);

        if (existingProduct is null)
        {
            return NotFound();
        }

        await _productService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);

        return NoContent();
    }
}
