using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using ProyectoBase.Api.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProyectoBase.Api.Swagger.Filters;

/// <summary>
/// Ensures that default error responses are documented for all operations.
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

        AddResponse(operation, context, StatusCodes.Status400BadRequest, "Bad request.");
        AddResponse(operation, context, StatusCodes.Status404NotFound, "Resource not found.");
        AddResponse(operation, context, StatusCodes.Status500InternalServerError, "Unexpected error.");
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
