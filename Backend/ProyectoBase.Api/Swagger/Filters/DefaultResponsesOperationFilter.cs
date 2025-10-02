using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using ProyectoBase.Api.Api.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProyectoBase.Api.Api.Swagger.Filters;

/// <summary>
/// Garantiza que las respuestas de error predeterminadas queden documentadas para todas las operaciones.
/// </summary>
public sealed class DefaultResponsesOperationFilter : IOperationFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation == null)
        {
            throw new ArgumentNullException(nameof(operation));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        AddResponse(operation, context, StatusCodes.Status400BadRequest, "Solicitud inválida.");
        AddResponse(operation, context, StatusCodes.Status404NotFound, "Recurso no encontrado.");
        AddResponse(operation, context, StatusCodes.Status500InternalServerError, "Error inesperado.");
    }

    private static void AddResponse(OpenApiOperation operation, OperationFilterContext context, int statusCode, string description)
    {
        var schema = context.SchemaGenerator.GenerateSchema(typeof(ErrorResponse), context.SchemaRepository);
        var response = new OpenApiResponse
        {
            Description = description,
            Content =
            {
                ["application/json"] = new OpenApiMediaType
                {
                    Schema = schema,
                },
            },
        };

        var key = statusCode.ToString(CultureInfo.InvariantCulture);

        operation.Responses[key] = response;
    }
}
