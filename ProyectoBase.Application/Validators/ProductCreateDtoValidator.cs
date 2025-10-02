using FluentValidation;
using ProyectoBase.Application.DTOs;

namespace ProyectoBase.Application.Validators
{
    /// <summary>
    /// Validator that ensures <see cref="ProductCreateDto"/> instances contain valid data.
    /// </summary>
    public class ProductCreateDtoValidator : AbstractValidator<ProductCreateDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCreateDtoValidator"/> class.
        /// </summary>
        public ProductCreateDtoValidator()
        {
            RuleFor(product => product.Name)
                .NotEmpty()
                .Length(2, 100);

            RuleFor(product => product.Description)
                .MaximumLength(500);

            RuleFor(product => product.Price)
                .GreaterThan(0)
                .LessThanOrEqualTo(999999.99m);

            RuleFor(product => product.Stock)
                .GreaterThanOrEqualTo(0);
        }
    }
}
