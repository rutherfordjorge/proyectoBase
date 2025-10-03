namespace ProyectoBase.Api.Domain;

/// <summary>
/// Contiene los códigos de error canónicos utilizados por el dominio.
/// </summary>
public static class DomainErrorCodes
{
    /// <summary>
    /// Error general que no puede clasificarse en una categoría más específica.
    /// </summary>
    public const string General = "dominio.error_general";

    /// <summary>
    /// Error que indica que la entidad solicitada no superó las validaciones de negocio.
    /// </summary>
    public const string Validation = "dominio.error_validacion";

    /// <summary>
    /// Error que indica que la entidad solicitada no fue encontrada.
    /// </summary>
    public const string NotFound = "dominio.error_no_encontrado";

    /// <summary>
    /// Error que indica que el identificador del producto es obligatorio.
    /// </summary>
    public const string ProductIdRequired = "dominio.producto.id_requerido";

    /// <summary>
    /// Error que indica que el inventario del producto no puede ser negativo.
    /// </summary>
    public const string ProductStockNegative = "dominio.producto.inventario_negativo";

    /// <summary>
    /// Error que indica que la cantidad para incrementar el inventario debe ser positiva.
    /// </summary>
    public const string ProductStockIncreaseQuantityNotPositive = "dominio.producto.inventario_incremento_invalido";

    /// <summary>
    /// Error que indica que la cantidad para disminuir el inventario debe ser positiva.
    /// </summary>
    public const string ProductStockDecreaseQuantityNotPositive = "dominio.producto.inventario_disminucion_invalida";

    /// <summary>
    /// Error que indica que la disminución solicitada excede el inventario disponible.
    /// </summary>
    public const string ProductStockDecreaseQuantityExceeds = "dominio.producto.inventario_disminucion_excede";

    /// <summary>
    /// Error que indica que la cantidad verificada en el inventario debe ser positiva.
    /// </summary>
    public const string ProductStockCheckQuantityPositive = "dominio.producto.inventario_verificacion_invalida";

    /// <summary>
    /// Error que indica que el precio del producto no puede ser negativo.
    /// </summary>
    public const string ProductPriceNegative = "dominio.producto.precio_negativo";

    /// <summary>
    /// Error que indica que el nombre del producto es obligatorio.
    /// </summary>
    public const string ProductNameMissing = "dominio.producto.nombre_faltante";

    /// <summary>
    /// Error que indica que la longitud del nombre del producto está fuera del rango permitido.
    /// </summary>
    public const string ProductNameLength = "dominio.producto.nombre_longitud_invalida";

    /// <summary>
    /// Error que indica que la longitud de la descripción del producto está fuera del rango permitido.
    /// </summary>
    public const string ProductDescriptionLength = "dominio.producto.descripcion_longitud_invalida";
}
