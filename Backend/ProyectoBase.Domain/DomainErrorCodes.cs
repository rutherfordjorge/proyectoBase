namespace ProyectoBase.Api.Domain;

/// <summary>
/// Contiene los códigos de error canónicos utilizados por el dominio.
/// </summary>
public static class DomainErrorCodes
{
    public const string General = "dominio.error_general";
    public const string Validation = "dominio.error_validacion";
    public const string NotFound = "dominio.error_no_encontrado";
    public const string ProductIdRequired = "dominio.producto.id_requerido";
    public const string ProductStockNegative = "dominio.producto.inventario_negativo";
    public const string ProductStockIncreaseQuantityNotPositive = "dominio.producto.inventario_incremento_invalido";
    public const string ProductStockDecreaseQuantityNotPositive = "dominio.producto.inventario_disminucion_invalida";
    public const string ProductStockDecreaseQuantityExceeds = "dominio.producto.inventario_disminucion_excede";
    public const string ProductStockCheckQuantityPositive = "dominio.producto.inventario_verificacion_invalida";
    public const string ProductPriceNegative = "dominio.producto.precio_negativo";
    public const string ProductNameMissing = "dominio.producto.nombre_faltante";
    public const string ProductNameLength = "dominio.producto.nombre_longitud_invalida";
    public const string ProductDescriptionLength = "dominio.producto.descripcion_longitud_invalida";
}
