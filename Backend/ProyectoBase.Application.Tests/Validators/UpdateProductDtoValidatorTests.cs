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
}
