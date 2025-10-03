using System;
using MediatR;

namespace ProyectoBase.Api.Application.Products.Commands.DeleteProduct;

/// <summary>
/// Representa la solicitud para eliminar un producto.
/// </summary>
public sealed record DeleteProductCommand(Guid Id) : IRequest;
