using FluentAssertions;
using ProyectoBase.Api.Domain.Exceptions;
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

        action.Should().Throw<ValidationException>()
            .WithMessage("El nombre del producto no puede estar vacío.");
    }

    [Fact]
    public void Create_ShouldThrowWhenValueIsTooShort()
    {
        var action = () => ProductName.Create("A");

        action.Should().Throw<ValidationException>()
            .WithMessage($"El nombre del producto debe tener entre {ProductName.MinLength} y {ProductName.MaxLength} caracteres.");
    }

    [Fact]
    public void Create_ShouldThrowWhenValueIsTooLong()
    {
        var value = new string('A', ProductName.MaxLength + 1);

        var action = () => ProductName.Create(value);

        action.Should().Throw<ValidationException>()
            .WithMessage($"El nombre del producto debe tener entre {ProductName.MinLength} y {ProductName.MaxLength} caracteres.");
    }
}
