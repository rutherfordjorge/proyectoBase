using FluentValidation;
using ProyectoBase.Application.DTOs;

namespace ProyectoBase.Application.Validators
{
    /// <summary>
    /// Ensures <see cref="ProductUpdateDto"/> instances contain valid data before being processed.
    /// </summary>
    public class UpdateProductDtoValidator : AbstractValidator<ProductUpdateDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateProductDtoValidator"/> class.
        /// </summary>
        public UpdateProductDtoValidator()
        {
            Include(new CreateProductDtoValidator());

            RuleFor(product => product.Id)
                .NotEmpty();
        }
    }
}
