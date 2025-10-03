using System;
using ProyectoBase.Api.Domain;
using ProyectoBase.Api.Domain.Exceptions;

namespace ProyectoBase.Api.Domain.ValueObjects
{
    /// <summary>
    /// Representa un valor monetario con precisión de dos decimales.
    /// </summary>
    public sealed class Money : IEquatable<Money>
    {
        private Money(decimal amount)
        {
            Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Obtiene el monto monetario.
        /// </summary>
        public decimal Amount { get; }

        /// <summary>
        /// Crea una instancia de <see cref="Money"/> asegurando que el monto sea válido.
        /// </summary>
        /// <param name="amount">Monto monetario a validar.</param>
        /// <returns>Una instancia válida de <see cref="Money"/>.</returns>
        /// <exception cref="ValidationException">Se produce cuando el monto es negativo.</exception>
        public static Money From(decimal amount)
        {
            if (amount < 0)
            {
                throw new ValidationException(DomainErrors.Product.PriceCannotBeNegative);
            }

            return new Money(amount);
        }

        /// <inheritdoc />
        public bool Equals(Money? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Amount == other.Amount;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return Equals(obj as Money);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Amount.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Amount.ToString("F2");
        }

        /// <summary>
        /// Conversión implícita a <see cref="decimal"/>.
        /// </summary>
        /// <param name="money">El valor monetario a convertir.</param>
        public static implicit operator decimal(Money money)
        {
            ArgumentNullException.ThrowIfNull(money);
            return money.Amount;
        }
    }
}
