using FluentAssertions;
using ProyectoBase.Api.Domain.Exceptions;
using ProyectoBase.Api.Domain;
using ProyectoBase.Api.Domain.ValueObjects;
using Xunit;

namespace ProyectoBase.Api.Domain.Tests.ValueObjects;

public class ProductNameTests
{
    [Fact]
    public void Create_ShouldTrimAndReturnProductName()
    {
        var productName = ProductName.Create("  Laptop  ");

        productName.Value.Should().Be("Laptop");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrowWhenValueIsMissing(string? value)
    {
        var action = () => ProductName.Create(value!);

        var expectedError = DomainErrors.Product.NameIsMissing;

        action.Should().Throw<ValidationException>()
            .Where(exception => exception.Code == expectedError.Code)
            .WithMessage(expectedError.Message);
    }

    [Fact]
    public void Create_ShouldThrowWhenValueIsTooShort()
    {
        var action = () => ProductName.Create("A");

        var expectedError = DomainErrors.Product.NameLengthIsInvalid(ProductName.MinLength, ProductName.MaxLength);

        action.Should().Throw<ValidationException>()
            .Where(exception => exception.Code == expectedError.Code)
            .WithMessage(expectedError.Message);
    }

    [Fact]
    public void Create_ShouldThrowWhenValueIsTooLong()
    {
        var value = new string('A', ProductName.MaxLength + 1);

        var action = () => ProductName.Create(value);

        var expectedError = DomainErrors.Product.NameLengthIsInvalid(ProductName.MinLength, ProductName.MaxLength);

        action.Should().Throw<ValidationException>()
            .Where(exception => exception.Code == expectedError.Code)
            .WithMessage(expectedError.Message);
    }
}
