using System;

namespace ProyectoBase.Api.Application.DTOs
{
    /// <summary>
    /// Representa los datos de producto devueltos por los servicios de la aplicación.
    /// </summary>
    public class ProductResponseDto
    {
        /// <summary>
        /// Obtiene o establece el identificador único del producto.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del producto.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Obtiene o establece la descripción opcional del producto.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Obtiene o establece el precio del producto.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Obtiene o establece el inventario disponible del producto.
        /// </summary>
        public int Stock { get; set; }
    }
}
