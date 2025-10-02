using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProyectoBase.Api.Swagger;

/// <summary>
/// Configura los documentos de Swagger según las versiones disponibles de la API.
/// </summary>
public sealed class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="ConfigureSwaggerOptions"/>.
    /// </summary>
    /// <param name="provider">El proveedor de descripciones de versiones de la API.</param>
    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    /// <inheritdoc />
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            var info = CreateInfoForApiVersion(description);
            options.SwaggerDoc(description.GroupName, info);
        }
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var info = new OpenApiInfo
        {
            Title = "API de ProyectoBase",
            Version = description.ApiVersion.ToString(),
            Description = "Documentación de la API de ProyectoBase.",
        };

        if (description.IsDeprecated)
        {
            info.Description += " Esta versión de la API se encuentra obsoleta.";
        }

        return info;
    }
}
