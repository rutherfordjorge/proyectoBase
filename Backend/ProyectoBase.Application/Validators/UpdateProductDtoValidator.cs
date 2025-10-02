using FluentValidation;
using ProyectoBase.Api.Application.DTOs;

namespace ProyectoBase.Api.Application.Validators
{
    /// <summary>
    /// Garantiza que las instancias de <see cref="ProductUpdateDto"/> contengan datos válidos antes de ser procesadas.
    /// </summary>
    public class UpdateProductDtoValidator : AbstractValidator<ProductUpdateDto>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="UpdateProductDtoValidator"/>.
        /// </summary>
        public UpdateProductDtoValidator()
        {
            Include(new CreateProductDtoValidator());

            RuleFor(product => product.Id)
                .NotEmpty().WithMessage("El identificador del producto es obligatorio.");
        }
    }
}
