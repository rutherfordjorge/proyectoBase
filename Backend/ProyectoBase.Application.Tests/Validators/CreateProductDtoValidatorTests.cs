using FluentValidation.TestHelper;
using ProyectoBase.Api.Application.DTOs;
using ProyectoBase.Api.Application.Validators;
using Xunit;

namespace ProyectoBase.Api.Application.Tests.Validators;

/// <summary>
/// Contains unit tests for <see cref="CreateProductDtoValidator"/>.
/// </summary>
public class CreateProductDtoValidatorTests
{
    private readonly CreateProductDtoValidator _validator = new();

    [Fact]
    public void Should_fail_when_name_is_empty()
    {
        var dto = new ProductCreateDto
        {
            Name = string.Empty,
            Price = 10,
            Stock = 1
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(product => product.Name);
    }

    [Fact]
    public void Should_fail_when_price_is_not_positive()
    {
        var dto = new ProductCreateDto
        {
            Name = "Valid name",
            Price = 0,
            Stock = 1
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(product => product.Price);
    }

    [Fact]
    public void Should_pass_when_dto_is_valid()
    {
        var dto = new ProductCreateDto
        {
            Name = "Valid name",
            Description = "Valid description",
            Price = 100,
            Stock = 10
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
