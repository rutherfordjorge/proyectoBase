using System;

namespace ProyectoBase.Application.DTOs
{
    /// <summary>
    /// Represents the product data returned by application services.
    /// </summary>
    public class ProductResponseDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the product.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the optional description of the product.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the price of the product.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the available stock of the product.
        /// </summary>
        public int Stock { get; set; }
    }
}
