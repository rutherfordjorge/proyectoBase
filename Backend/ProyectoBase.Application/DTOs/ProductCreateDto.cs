using System.ComponentModel.DataAnnotations;

namespace ProyectoBase.Application.DTOs
{
    /// <summary>
    /// Representa los datos necesarios para crear un nuevo producto.
    /// </summary>
    public class ProductCreateDto
    {
        /// <summary>
        /// Obtiene o establece el nombre del producto.
        /// </summary>
        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Obtiene o establece la descripción del producto.
        /// </summary>
        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string? Description { get; set; }

        /// <summary>
        /// Obtiene o establece el precio del producto.
        /// </summary>
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe estar entre 0,01 y 999.999,99.")]
        public decimal Price { get; set; }

        /// <summary>
        /// Obtiene o establece el inventario disponible del producto.
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "El inventario debe ser un número mayor o igual que cero.")]
        public int Stock { get; set; }
    }
}
