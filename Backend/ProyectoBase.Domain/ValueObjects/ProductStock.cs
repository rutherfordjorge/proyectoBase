using System;
using ProyectoBase.Api.Domain;
using ProyectoBase.Api.Domain.Exceptions;

namespace ProyectoBase.Api.Domain.ValueObjects
{
    /// <summary>
    /// Representa el inventario disponible de un producto asegurando sus reglas de dominio.
    /// </summary>
    public sealed class ProductStock : IEquatable<ProductStock>
    {
        private ProductStock(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Obtiene la cantidad de unidades disponibles en inventario.
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Crea una nueva instancia de <see cref="ProductStock"/> validando que el inventario no sea negativo.
        /// </summary>
        /// <param name="value">Cantidad de unidades disponibles.</param>
        /// <returns>Una instancia válida de <see cref="ProductStock"/>.</returns>
        /// <exception cref="ValidationException">Se produce cuando el valor es negativo.</exception>
        public static ProductStock Create(int value)
        {
            if (value < 0)
            {
                throw new ValidationException(DomainErrors.Product.StockCannotBeNegative);
            }

            return new ProductStock(value);
        }

        /// <summary>
        /// Incrementa el inventario en la cantidad proporcionada.
        /// </summary>
        /// <param name="quantity">Cantidad a incrementar.</param>
        /// <returns>Una nueva instancia de <see cref="ProductStock"/> con la cantidad actualizada.</returns>
        /// <exception cref="ValidationException">Se produce cuando la cantidad es menor o igual que cero.</exception>
        public ProductStock Increase(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ValidationException(DomainErrors.Product.StockIncreaseQuantityMustBePositive);
            }

            var result = checked(Value + quantity);
            return new ProductStock(result);
        }

        /// <summary>
        /// Disminuye el inventario en la cantidad proporcionada.
        /// </summary>
        /// <param name="quantity">Cantidad a disminuir.</param>
        /// <returns>Una nueva instancia de <see cref="ProductStock"/> con la cantidad actualizada.</returns>
        /// <exception cref="ValidationException">Se produce cuando la cantidad es menor o igual que cero o excede el inventario disponible.</exception>
        public ProductStock Decrease(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ValidationException(DomainErrors.Product.StockDecreaseQuantityMustBePositive);
            }

            if (quantity > Value)
            {
                throw new ValidationException(DomainErrors.Product.StockDecreaseQuantityExceedsAvailable);
            }

            return new ProductStock(Value - quantity);
        }

        /// <inheritdoc />
        public bool Equals(ProductStock? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Value == other.Value;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return Equals(obj as ProductStock);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Value.ToString();
        }

        /// <summary>
        /// Conversión implícita a <see cref="int"/>.
        /// </summary>
        /// <param name="stock">El inventario a convertir.</param>
        public static implicit operator int(ProductStock stock)
        {
            ArgumentNullException.ThrowIfNull(stock);
            return stock.Value;
        }
    }
}
