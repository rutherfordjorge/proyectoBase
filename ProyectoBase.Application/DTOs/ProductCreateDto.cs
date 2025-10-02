using System.ComponentModel.DataAnnotations;

namespace ProyectoBase.Application.DTOs
{
    /// <summary>
    /// Represents the data required to create a new product.
    /// </summary>
    public class ProductCreateDto
    {
        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        [StringLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the price of the product.
        /// </summary>
        [Range(0.01, 999999.99)]
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the available stock of the product.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
    }
}
