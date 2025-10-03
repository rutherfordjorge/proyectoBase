using System;

namespace ProyectoBase.Api.Domain;

/// <summary>
/// Catálogo centralizado de errores de dominio en español.
/// </summary>
public static class DomainErrors
{
    public static class General
    {
        public static DomainError Validation => new(DomainErrorCodes.Validation, "Los datos proporcionados no son válidos.");

        public static DomainError NotFound => new(DomainErrorCodes.NotFound, "El recurso solicitado no fue encontrado.");
    }

    public static class Product
    {
        public static DomainError IdRequired => new(DomainErrorCodes.ProductIdRequired, "Se debe proporcionar el identificador del producto.");

        public static DomainError StockCannotBeNegative => new(DomainErrorCodes.ProductStockNegative, "El inventario del producto no puede ser negativo.");

        public static DomainError StockIncreaseQuantityMustBePositive => new(DomainErrorCodes.ProductStockIncreaseQuantityNotPositive, "La cantidad a incrementar debe ser mayor que cero.");

        public static DomainError StockDecreaseQuantityMustBePositive => new(DomainErrorCodes.ProductStockDecreaseQuantityNotPositive, "La cantidad a disminuir debe ser mayor que cero.");

        public static DomainError StockDecreaseQuantityExceedsAvailable => new(DomainErrorCodes.ProductStockDecreaseQuantityExceeds, "La cantidad a disminuir excede el inventario disponible.");

        public static DomainError StockQuantityToCheckMustBePositive => new(DomainErrorCodes.ProductStockCheckQuantityPositive, "La cantidad a verificar debe ser mayor que cero.");

        public static DomainError PriceCannotBeNegative => new(DomainErrorCodes.ProductPriceNegative, "El precio del producto no puede ser negativo.");

        public static DomainError NameIsMissing => new(DomainErrorCodes.ProductNameMissing, "El nombre del producto no puede estar vacío.");

        public static DomainError NameLengthIsInvalid(int minLength, int maxLength) =>
            new(DomainErrorCodes.ProductNameLength, $"El nombre del producto debe tener entre {minLength} y {maxLength} caracteres.");

        public static DomainError DescriptionLengthIsInvalid(int maxLength) =>
            new(DomainErrorCodes.ProductDescriptionLength, $"La descripción del producto no puede superar los {maxLength} caracteres.");

        public static DomainError NotFound(Guid productId) =>
            new(DomainErrorCodes.NotFound, $"No se encontró el producto con identificador '{productId}'.");
    }
}
