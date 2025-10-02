using FluentValidation;
using ProyectoBase.Application.DTOs;

namespace ProyectoBase.Application.Validators
{
    /// <summary>
    /// Validator that ensures <see cref="ProductUpdateDto"/> instances contain valid data.
    /// </summary>
    public class ProductUpdateDtoValidator : AbstractValidator<ProductUpdateDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductUpdateDtoValidator"/> class.
        /// </summary>
        public ProductUpdateDtoValidator()
        {
            Include(new ProductCreateDtoValidator());

            RuleFor(product => product.Id)
                .NotEmpty();
        }
    }
}
