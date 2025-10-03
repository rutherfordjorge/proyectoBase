using MediatR;
using ProyectoBase.Api.Application.DTOs;

namespace ProyectoBase.Api.Application.Products.Commands.UpdateProduct;

/// <summary>
/// Representa la solicitud para actualizar un producto existente.
/// </summary>
public sealed record UpdateProductCommand(ProductUpdateDto Product) : IRequest<ProductResponseDto>;
