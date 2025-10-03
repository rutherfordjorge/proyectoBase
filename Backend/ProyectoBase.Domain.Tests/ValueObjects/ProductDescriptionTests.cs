using FluentAssertions;
using ProyectoBase.Api.Domain.Exceptions;
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

        action.Should().Throw<ValidationException>()
            .WithMessage($"La descripción del producto no puede superar los {ProductDescription.MaxLength} caracteres.");
    }
}
