using System.Collections.Generic;
using MediatR;
using ProyectoBase.Api.Application.DTOs;

namespace ProyectoBase.Api.Application.Products.Queries.GetAllProducts;

/// <summary>
/// Representa la solicitud para obtener el listado completo de productos.
/// </summary>
public sealed record GetAllProductsQuery() : IRequest<IReadOnlyCollection<ProductResponseDto>>;
