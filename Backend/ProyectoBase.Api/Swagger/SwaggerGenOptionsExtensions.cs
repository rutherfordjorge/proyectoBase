using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProyectoBase.Api.Swagger;

/// <summary>
/// Provides helper methods to configure Swagger generation.
/// </summary>
public static class SwaggerGenOptionsExtensions
{
    /// <summary>
    /// Maps enumeration values to the <c>code</c> representation defined by <see cref="EnumMemberAttribute"/>.
    /// </summary>
    /// <param name="options">The Swagger generation options.</param>
    /// <param name="assemblies">Assemblies that may contain enums to map.</param>
    public static void MapCodeEnumsFromAssemblies(this SwaggerGenOptions options, params Assembly[] assemblies)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (assemblies == null || assemblies.Length == 0)
        {
            return;
        }

        foreach (var enumType in assemblies.SelectMany(EnumerateEnumTypes).Distinct())
        {
            options.SchemaGeneratorOptions.CustomTypeMappings.TryAdd(enumType, () => CreateSchema(enumType));
        }
    }

    private static IEnumerable<Type> EnumerateEnumTypes(Assembly assembly)
    {
        return assembly
            .GetExportedTypes()
            .Where(static type => type.IsEnum);
    }

    private static OpenApiSchema CreateSchema(Type enumType)
    {
        var values = Enum.GetValues(enumType)
            .Cast<object>()
            .Select(value => new OpenApiString(GetEnumMemberValue(enumType, value)))
            .Cast<IOpenApiAny>()
            .ToList();

        return new OpenApiSchema
        {
            Type = "string",
            Enum = values,
            Description = $"Enumeration of {enumType.Name} values.",
        };
    }

    private static string GetEnumMemberValue(Type enumType, object value)
    {
        var name = Enum.GetName(enumType, value) ?? value.ToString() ?? string.Empty;
        var member = enumType.GetMember(name).FirstOrDefault();
        var enumMember = member?.GetCustomAttribute<EnumMemberAttribute>();

        return enumMember?.Value ?? name;
    }
}
