using System;
using ProyectoBase.Api.Domain;
using ProyectoBase.Api.Domain.Exceptions;

namespace ProyectoBase.Api.Domain.ValueObjects
{
    /// <summary>
    /// Representa la descripción opcional de un producto.
    /// </summary>
    public sealed class ProductDescription : IEquatable<ProductDescription>
    {
        /// <summary>
        /// Longitud máxima permitida para una descripción de producto.
        /// </summary>
        public const int MaxLength = 500;

        private ProductDescription(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Obtiene el valor subyacente de la descripción.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Crea una nueva instancia de <see cref="ProductDescription"/> validando las reglas de negocio.
        /// </summary>
        /// <param name="value">Valor opcional a normalizar.</param>
        /// <returns>Una instancia válida de <see cref="ProductDescription"/> o <c>null</c> cuando el valor no contiene información.</returns>
        /// <exception cref="ValidationException">Se produce cuando la descripción supera la longitud máxima permitida.</exception>
        public static ProductDescription? Create(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var normalized = value.Trim();

            if (normalized.Length > MaxLength)
            {
                throw new ValidationException(DomainErrors.Product.DescriptionLengthIsInvalid(MaxLength));
            }

            return new ProductDescription(normalized);
        }

        /// <inheritdoc />
        public bool Equals(ProductDescription? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return Equals(obj as ProductDescription);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(Value);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// Conversión implícita a <see cref="string"/>.
        /// </summary>
        /// <param name="description">La descripción de producto a convertir.</param>
        public static implicit operator string(ProductDescription description)
        {
            ArgumentNullException.ThrowIfNull(description);
            return description.Value;
        }
    }
}
