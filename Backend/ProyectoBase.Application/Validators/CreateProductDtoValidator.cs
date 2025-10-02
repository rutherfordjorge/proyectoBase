using FluentValidation;
using ProyectoBase.Application.DTOs;

namespace ProyectoBase.Application.Validators
{
    /// <summary>
    /// Garantiza que las instancias de <see cref="ProductCreateDto"/> contengan datos válidos antes de ser procesadas.
    /// </summary>
    public class CreateProductDtoValidator : AbstractValidator<ProductCreateDto>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="CreateProductDtoValidator"/>.
        /// </summary>
        public CreateProductDtoValidator()
        {
            RuleFor(product => product.Name)
                .NotEmpty().WithMessage("El nombre del producto es obligatorio.")
                .Length(2, 100).WithMessage("El nombre debe tener entre 2 y 100 caracteres.");

            RuleFor(product => product.Description)
                .MaximumLength(500).WithMessage("La descripción no puede superar los 500 caracteres.");

            RuleFor(product => product.Price)
                .GreaterThan(0).WithMessage("El precio debe ser mayor que cero.")
                .LessThanOrEqualTo(999999.99m).WithMessage("El precio no puede superar 999.999,99.");

            RuleFor(product => product.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("El inventario debe ser un número mayor o igual que cero.");
        }
    }
}
