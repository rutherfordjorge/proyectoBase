using System;

namespace ProyectoBase.Api.Domain;

/// <summary>
/// Catálogo centralizado de errores de dominio en español.
/// </summary>
public static class DomainErrors
{
    /// <summary>
    /// Define errores de dominio comunes que aplican de forma general para el sistema.
    /// </summary>
    public static class General
    {
        /// <summary>
        /// Obtiene un error que representa una validación fallida de datos.
        /// </summary>
        public static DomainError Validation => new(DomainErrorCodes.Validation, "Los datos proporcionados no son válidos.");

        /// <summary>
        /// Obtiene un error que indica que el recurso solicitado no existe.
        /// </summary>
        public static DomainError NotFound => new(DomainErrorCodes.NotFound, "El recurso solicitado no fue encontrado.");
    }

    /// <summary>
    /// Contiene errores de dominio relacionados con las operaciones sobre productos.
    /// </summary>
    public static class Product
    {
        /// <summary>
        /// Obtiene un error cuando no se proporciona el identificador del producto.
        /// </summary>
        public static DomainError IdRequired => new(DomainErrorCodes.ProductIdRequired, "Se debe proporcionar el identificador del producto.");

        /// <summary>
        /// Obtiene un error cuando se intenta asignar un inventario negativo a un producto.
        /// </summary>
        public static DomainError StockCannotBeNegative => new(DomainErrorCodes.ProductStockNegative, "El inventario del producto no puede ser negativo.");

        /// <summary>
        /// Obtiene un error cuando la cantidad para incrementar el inventario no es positiva.
        /// </summary>
        public static DomainError StockIncreaseQuantityMustBePositive => new(DomainErrorCodes.ProductStockIncreaseQuantityNotPositive, "La cantidad a incrementar debe ser mayor que cero.");

        /// <summary>
        /// Obtiene un error cuando la cantidad para disminuir el inventario no es positiva.
        /// </summary>
        public static DomainError StockDecreaseQuantityMustBePositive => new(DomainErrorCodes.ProductStockDecreaseQuantityNotPositive, "La cantidad a disminuir debe ser mayor que cero.");

        /// <summary>
        /// Obtiene un error cuando la cantidad a disminuir del inventario excede la disponible.
        /// </summary>
        public static DomainError StockDecreaseQuantityExceedsAvailable => new(DomainErrorCodes.ProductStockDecreaseQuantityExceeds, "La cantidad a disminuir excede el inventario disponible.");

        /// <summary>
        /// Obtiene un error cuando la cantidad a verificar en el inventario no es positiva.
        /// </summary>
        public static DomainError StockQuantityToCheckMustBePositive => new(DomainErrorCodes.ProductStockCheckQuantityPositive, "La cantidad a verificar debe ser mayor que cero.");

        /// <summary>
        /// Obtiene un error cuando se intenta asignar un precio negativo a un producto.
        /// </summary>
        public static DomainError PriceCannotBeNegative => new(DomainErrorCodes.ProductPriceNegative, "El precio del producto no puede ser negativo.");

        /// <summary>
        /// Obtiene un error cuando el nombre del producto no es proporcionado.
        /// </summary>
        public static DomainError NameIsMissing => new(DomainErrorCodes.ProductNameMissing, "El nombre del producto no puede estar vacío.");

        /// <summary>
        /// Obtiene un error cuando la longitud del nombre del producto no se encuentra dentro de los límites permitidos.
        /// </summary>
        /// <param name="minLength">La longitud mínima permitida para el nombre del producto.</param>
        /// <param name="maxLength">La longitud máxima permitida para el nombre del producto.</param>
        public static DomainError NameLengthIsInvalid(int minLength, int maxLength) =>
            new(DomainErrorCodes.ProductNameLength, $"El nombre del producto debe tener entre {minLength} y {maxLength} caracteres.");

        /// <summary>
        /// Obtiene un error cuando la descripción del producto supera la longitud máxima permitida.
        /// </summary>
        /// <param name="maxLength">La longitud máxima permitida para la descripción del producto.</param>
        public static DomainError DescriptionLengthIsInvalid(int maxLength) =>
            new(DomainErrorCodes.ProductDescriptionLength, $"La descripción del producto no puede superar los {maxLength} caracteres.");

        /// <summary>
        /// Obtiene un error cuando no se encuentra un producto con el identificador proporcionado.
        /// </summary>
        /// <param name="productId">El identificador del producto buscado.</param>
        public static DomainError NotFound(Guid productId) =>
            new(DomainErrorCodes.NotFound, $"No se encontró el producto con identificador '{productId}'.");
    }
}
