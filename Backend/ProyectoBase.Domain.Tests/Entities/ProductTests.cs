using System;
using FluentAssertions;
using ProyectoBase.Api.Domain;
using ProyectoBase.Api.Domain.Entities;
using ProyectoBase.Api.Domain.Exceptions;
using Xunit;

namespace ProyectoBase.Api.Domain.Tests.Entities;

public class ProductTests
{
    [Fact]
    public void Constructor_ShouldNormalizeAndAssignProvidedValues()
    {
        var id = Guid.NewGuid();
        const string name = "  Gaming Laptop  ";
        const string description = "  Powerful machine  ";
        const decimal price = 1999.999m;
        const int stock = 5;

        var product = new Product(id, name, price, stock, description);

        product.Id.Should().Be(id);
        product.Name.Value.Should().Be("Gaming Laptop");
        product.Description!.Value.Should().Be("Powerful machine");
        product.Price.Amount.Should().Be(2000.00m);
        product.Stock.Value.Should().Be(stock);
    }

    [Fact]
    public void Constructor_ShouldThrowWhenIdIsEmpty()
    {
        // Invariante: el identificador del producto no puede ser Guid.Empty.
        var action = () => new Product(Guid.Empty, "Valid", 10m, 5, "Description");

        var expectedError = DomainErrors.Product.IdRequired;

        action.Should().Throw<ValidationException>()
            .Where(exception => exception.Code == expectedError.Code)
            .WithMessage(expectedError.Message);
    }

    [Fact]
    public void Constructor_ShouldThrowWhenNameIsNull()
    {
        // Invariante: el nombre del producto no puede ser nulo ni vacío.
        var action = () => new Product(Guid.NewGuid(), null!, 10m, 5, "Description");

        var expectedError = DomainErrors.Product.NameIsMissing;

        action.Should().Throw<ValidationException>()
            .Where(exception => exception.Code == expectedError.Code)
            .WithMessage(expectedError.Message);
    }

    [Fact]
    public void Constructor_ShouldThrowWhenPriceIsNegative()
    {
        // Invariante: el precio debe ser cero o positivo.
        var action = () => new Product(Guid.NewGuid(), "Valid", -1m, 5, "Description");

        var expectedError = DomainErrors.Product.PriceCannotBeNegative;

        action.Should().Throw<ValidationException>()
            .Where(exception => exception.Code == expectedError.Code)
            .WithMessage(expectedError.Message);
    }

    [Fact]
    public void Constructor_ShouldThrowWhenStockIsNegative()
    {
        // Invariante: el inventario inicial no puede ser negativo.
        var action = () => new Product(Guid.NewGuid(), "Valid", 10m, -1, "Description");

        var expectedError = DomainErrors.Product.StockCannotBeNegative;

        action.Should().Throw<ValidationException>()
            .Where(exception => exception.Code == expectedError.Code)
            .WithMessage(expectedError.Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void IncreaseStock_ShouldThrowWhenQuantityIsNotPositive(int quantity)
    {
        var product = CreateProduct();

        var action = () => product.IncreaseStock(quantity);

        var expectedError = DomainErrors.Product.StockIncreaseQuantityMustBePositive;

        action.Should().Throw<ValidationException>()
            .Where(exception => exception.Code == expectedError.Code)
            .WithMessage(expectedError.Message);
    }

    [Fact]
    public void ChangePrice_ShouldThrowWhenAmountIsNegative()
    {
        // Invariante: el precio siempre debe permanecer mayor o igual que cero.
        var product = CreateProduct();

        var action = () => product.ChangePrice(-5m);

        var expectedError = DomainErrors.Product.PriceCannotBeNegative;

        action.Should().Throw<ValidationException>()
            .Where(exception => exception.Code == expectedError.Code)
            .WithMessage(expectedError.Message);
    }

    [Theory]
    [InlineData(-3)]
    [InlineData(0)]
    public void DecreaseStock_ShouldThrowWhenQuantityIsNotPositive(int quantity)
    {
        // Invariante: la disminución de inventario requiere cantidades estrictamente positivas.
        var product = CreateProduct();

        var action = () => product.DecreaseStock(quantity);

        var expectedError = DomainErrors.Product.StockDecreaseQuantityMustBePositive;

        action.Should().Throw<ValidationException>()
            .Where(exception => exception.Code == expectedError.Code)
            .WithMessage(expectedError.Message);
    }

    [Fact]
    public void DecreaseStock_ShouldThrowWhenQuantityExceedsAvailableStock()
    {
        var product = CreateProduct(stock: 2);

        var action = () => product.DecreaseStock(5);

        var expectedError = DomainErrors.Product.StockDecreaseQuantityExceedsAvailable;

        action.Should().Throw<ValidationException>()
            .Where(exception => exception.Code == expectedError.Code)
            .WithMessage(expectedError.Message);
    }

    private static Product CreateProduct(int stock = 10)
    {
        return new Product(Guid.NewGuid(), "Sample", 10m, stock, "Description");
    }
}
