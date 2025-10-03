using System.Text.Json;

namespace ProyectoBase.Api.Application.Products.Common;

/// <summary>
/// Representa los valores constantes relacionados con la caché de productos.
/// </summary>
public static class ProductCacheSettings
{
    /// <summary>
    /// Obtiene la clave utilizada para almacenar el listado de productos en caché.
    /// </summary>
    public const string AllProductsCacheKey = "products:all";

    /// <summary>
    /// Obtiene las opciones de serialización empleadas para almacenar los productos en caché.
    /// </summary>
    public static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
}
