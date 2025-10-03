using MediatR;
using ProyectoBase.Api.Application.DTOs;

namespace ProyectoBase.Api.Application.Products.Commands.CreateProduct;

/// <summary>
/// Representa la solicitud para crear un nuevo producto.
/// </summary>
public sealed record CreateProductCommand(ProductCreateDto Product) : IRequest<ProductResponseDto>;
