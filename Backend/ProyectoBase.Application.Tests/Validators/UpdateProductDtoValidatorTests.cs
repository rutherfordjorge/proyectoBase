using FluentValidation.TestHelper;
using ProyectoBase.Api.Application.DTOs;
using ProyectoBase.Api.Application.Validators;
using Xunit;

namespace ProyectoBase.Api.Application.Tests.Validators;

/// <summary>
/// Contains unit tests for <see cref="UpdateProductDtoValidator"/>.
/// </summary>
public class UpdateProductDtoValidatorTests
{
    private readonly UpdateProductDtoValidator _validator = new();

    [Fact]
    public void Should_fail_when_identifier_is_empty()
    {
        var dto = new ProductUpdateDto
        {
            Id = Guid.Empty,
            Name = "Valid name",
            Price = 10,
            Stock = 1
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(product => product.Id);
    }

    [Fact]
    public void Should_inherit_rules_from_create_validator()
    {
        var dto = new ProductUpdateDto
        {
            Id = Guid.NewGuid(),
            Name = string.Empty,
            Price = 10,
            Stock = 1
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(product => product.Name);
    }

    [Fact]
    public void Should_fail_when_name_is_shorter_than_minimum_allowed()
    {
        var dto = new ProductUpdateDto
        {
            Id = Guid.NewGuid(),
            Name = "A",
            Price = 10,
            Stock = 1
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(product => product.Name)
            .WithErrorMessage("El nombre debe tener entre 2 y 100 caracteres.")
            .WithErrorCode("LengthValidator");
    }

    [Fact]
    public void Should_fail_when_name_is_longer_than_maximum_allowed()
    {
        var dto = new ProductUpdateDto
        {
            Id = Guid.NewGuid(),
            Name = new string('A', 101),
            Price = 10,
            Stock = 1
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(product => product.Name)
            .WithErrorMessage("El nombre debe tener entre 2 y 100 caracteres.")
            .WithErrorCode("LengthValidator");
    }

    [Fact]
    public void Should_fail_when_description_exceeds_maximum_length()
    {
        var dto = new ProductUpdateDto
        {
            Id = Guid.NewGuid(),
            Name = "Valid name",
            Description = new string('D', 501),
            Price = 10,
            Stock = 1
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(product => product.Description)
            .WithErrorMessage("La descripción no puede superar los 500 caracteres.")
            .WithErrorCode("MaximumLengthValidator");
    }

    [Fact]
    public void Should_fail_when_price_exceeds_maximum_allowed()
    {
        var dto = new ProductUpdateDto
        {
            Id = Guid.NewGuid(),
            Name = "Valid name",
            Price = 1_000_000m,
            Stock = 1
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(product => product.Price)
            .WithErrorMessage("El precio no puede superar 999.999,99.")
            .WithErrorCode("LessThanOrEqualValidator");
    }

    [Fact]
    public void Should_fail_when_stock_is_negative()
    {
        var dto = new ProductUpdateDto
        {
            Id = Guid.NewGuid(),
            Name = "Valid name",
            Price = 10,
            Stock = -1
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(product => product.Stock)
            .WithErrorMessage("El inventario debe ser un número mayor o igual que cero.")
            .WithErrorCode("GreaterThanOrEqualValidator");
    }
}
