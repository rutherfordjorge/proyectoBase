using System;
using ProyectoBase.Api.Domain.Exceptions;

namespace ProyectoBase.Api.Domain.ValueObjects
{
    /// <summary>
    /// Representa el nombre de un producto asegurando sus invariantes de dominio.
    /// </summary>
    public sealed class ProductName : IEquatable<ProductName>
    {
        /// <summary>
        /// Longitud mínima permitida para un nombre de producto.
        /// </summary>
        public const int MinLength = 2;

        /// <summary>
        /// Longitud máxima permitida para un nombre de producto.
        /// </summary>
        public const int MaxLength = 100;

        private ProductName(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Obtiene el valor subyacente del nombre.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Crea una nueva instancia de <see cref="ProductName"/> validando las reglas de negocio correspondientes.
        /// </summary>
        /// <param name="value">Valor a normalizar y validar.</param>
        /// <returns>Una instancia válida de <see cref="ProductName"/>.</returns>
        /// <exception cref="ValidationException">Se produce cuando el valor es nulo, vacío o no cumple las longitudes requeridas.</exception>
        public static ProductName Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValidationException("El nombre del producto no puede estar vacío.");
            }

            var normalized = value.Trim();

            if (normalized.Length < MinLength || normalized.Length > MaxLength)
            {
                throw new ValidationException($"El nombre del producto debe tener entre {MinLength} y {MaxLength} caracteres.");
            }

            return new ProductName(normalized);
        }

        /// <inheritdoc />
        public bool Equals(ProductName? other)
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
            return Equals(obj as ProductName);
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
        /// <param name="name">El nombre de producto a convertir.</param>
        public static implicit operator string(ProductName name)
        {
            ArgumentNullException.ThrowIfNull(name);
            return name.Value;
        }
    }
}
