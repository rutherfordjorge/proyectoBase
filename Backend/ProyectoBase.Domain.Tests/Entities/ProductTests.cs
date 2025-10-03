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
