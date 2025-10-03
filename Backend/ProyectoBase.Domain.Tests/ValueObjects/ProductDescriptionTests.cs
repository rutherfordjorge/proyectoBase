using FluentAssertions;
using ProyectoBase.Api.Domain.Exceptions;
using ProyectoBase.Api.Domain;
using ProyectoBase.Api.Domain.ValueObjects;
using Xunit;

namespace ProyectoBase.Api.Domain.Tests.ValueObjects;

public class ProductDescriptionTests
{
    [Fact]
    public void Create_ShouldReturnNullWhenValueIsEmpty()
    {
        var description = ProductDescription.Create("   ");

        description.Should().BeNull();
    }

    [Fact]
    public void Create_ShouldTrimAndReturnDescription()
    {
        var description = ProductDescription.Create("  Powerful  ");

        description!.Value.Should().Be("Powerful");
    }

    [Fact]
    public void Create_ShouldThrowWhenValueExceedsMaxLength()
    {
        var value = new string('A', ProductDescription.MaxLength + 1);

        var action = () => ProductDescription.Create(value);

        var expectedError = DomainErrors.Product.DescriptionLengthIsInvalid(ProductDescription.MaxLength);

        action.Should().Throw<ValidationException>()
            .Where(exception => exception.Code == expectedError.Code)
            .WithMessage(expectedError.Message);
    }
}
