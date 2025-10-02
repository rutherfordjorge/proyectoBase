using System;
using System.ComponentModel.DataAnnotations;

namespace ProyectoBase.Application.DTOs
{
    /// <summary>
    /// Represents the information required to update an existing product.
    /// </summary>
    public class ProductUpdateDto : ProductCreateDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the product to update.
        /// </summary>
        [Required]
        public Guid Id { get; set; }
    }
}
