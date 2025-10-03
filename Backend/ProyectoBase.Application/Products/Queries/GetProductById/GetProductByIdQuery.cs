using System;
using MediatR;
using ProyectoBase.Api.Application.DTOs;

namespace ProyectoBase.Api.Application.Products.Queries.GetProductById;

/// <summary>
/// Representa la solicitud para obtener un producto por su identificador.
/// </summary>
public sealed record GetProductByIdQuery(Guid Id) : IRequest<ProductResponseDto?>;
