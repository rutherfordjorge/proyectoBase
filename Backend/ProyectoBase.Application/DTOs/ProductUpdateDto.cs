using System;
using System.ComponentModel.DataAnnotations;

namespace ProyectoBase.Api.Application.DTOs
{
    /// <summary>
    /// Representa la información necesaria para actualizar un producto existente.
    /// </summary>
    public class ProductUpdateDto : ProductCreateDto
    {
        /// <summary>
        /// Obtiene o establece el identificador único del producto que se desea actualizar.
        /// </summary>
        [Required(ErrorMessage = "El identificador del producto es obligatorio.")]
        public Guid Id { get; set; }
    }
}
