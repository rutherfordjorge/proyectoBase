using FluentAssertions;
using ProyectoBase.Api.Domain.Exceptions;
using ProyectoBase.Api.Domain.ValueObjects;
using Xunit;

namespace ProyectoBase.Api.Domain.Tests.ValueObjects;

public class ProductStockTests
{
    [Fact]
    public void Create_ShouldReturnStockWhenValueIsValid()
    {
        var stock = ProductStock.Create(5);

        stock.Value.Should().Be(5);
    }

    [Fact]
    public void Create_ShouldThrowWhenValueIsNegative()
    {
        var action = () => ProductStock.Create(-1);

        action.Should().Throw<ValidationException>()
            .WithMessage("El inventario del producto no puede ser negativo.");
    }

    [Fact]
    public void Increase_ShouldReturnNewInstanceWithIncrementedValue()
    {
        var stock = ProductStock.Create(3);

        var result = stock.Increase(2);

        result.Value.Should().Be(5);
    }

    [Fact]
    public void Increase_ShouldThrowWhenQuantityIsNotPositive()
    {
        var stock = ProductStock.Create(3);

        var action = () => stock.Increase(0);

        action.Should().Throw<ValidationException>()
            .WithMessage("La cantidad a incrementar debe ser mayor que cero.");
    }

    [Fact]
    public void Decrease_ShouldReturnNewInstanceWithDecrementedValue()
    {
        var stock = ProductStock.Create(5);

        var result = stock.Decrease(2);

        result.Value.Should().Be(3);
    }

    [Fact]
    public void Decrease_ShouldThrowWhenQuantityExceedsValue()
    {
        var stock = ProductStock.Create(2);

        var action = () => stock.Decrease(3);

        action.Should().Throw<ValidationException>()
            .WithMessage("La cantidad a disminuir excede el inventario disponible.");
    }
}
