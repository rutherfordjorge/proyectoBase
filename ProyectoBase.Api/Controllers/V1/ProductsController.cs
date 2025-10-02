using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoBase.Application.Abstractions;
using ProyectoBase.Application.DTOs;

namespace ProyectoBase.Api.Controllers.V1;

/// <summary>
/// Exposes endpoints to manage products resources.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductsController"/> class.
    /// </summary>
    /// <param name="productService">The service responsible for product operations.</param>
    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Retrieves the complete list of available products.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A collection containing the requested products.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ProductResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<ProductResponseDto>>> GetProductsAsync(CancellationToken cancellationToken)
    {
        var products = await _productService.GetAllAsync(cancellationToken).ConfigureAwait(false);

        return Ok(products);
    }

    /// <summary>
    /// Retrieves a specific product by its unique identifier.
    /// </summary>
    /// <param name="id">The identifier of the product to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The product that matches the provided identifier.</returns>
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
    /// Creates a new product using the provided information.
    /// </summary>
    /// <param name="product">The product information.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The representation of the created product.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductResponseDto>> CreateProductAsync([FromBody] ProductCreateDto product, CancellationToken cancellationToken)
    {
        var createdProduct = await _productService.CreateAsync(product, cancellationToken).ConfigureAwait(false);
        var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";

        return CreatedAtAction(nameof(GetProductByIdAsync), new { id = createdProduct.Id, version }, createdProduct);
    }

    /// <summary>
    /// Updates the product that matches the provided identifier.
    /// </summary>
    /// <param name="id">The identifier of the product to update.</param>
    /// <param name="product">The product data to apply.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The representation of the updated product.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponseDto>> UpdateProductAsync(Guid id, [FromBody] ProductUpdateDto product, CancellationToken cancellationToken)
    {
        if (product.Id != Guid.Empty && product.Id != id)
        {
            return BadRequest("The provided identifier does not match the route value.");
        }

        product.Id = id;

        var updatedProduct = await _productService.UpdateAsync(product, cancellationToken).ConfigureAwait(false);

        return Ok(updatedProduct);
    }

    /// <summary>
    /// Deletes the product that matches the provided identifier.
    /// </summary>
    /// <param name="id">The identifier of the product to delete.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A response indicating the result of the deletion.</returns>
    [HttpDelete("{id:guid}")]
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
