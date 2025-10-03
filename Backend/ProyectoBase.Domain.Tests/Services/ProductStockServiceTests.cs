using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using ProyectoBase.Api.Domain;
using ProyectoBase.Api.Domain.Entities;
using ProyectoBase.Api.Domain.Exceptions;
using ProyectoBase.Api.Domain.Repositories;
using ProyectoBase.Api.Domain.Services;
using Xunit;

namespace ProyectoBase.Api.Domain.Tests.Services;

public class ProductStockServiceTests
{
    private readonly Mock<IProductReadRepository> _repositoryMock = new();
    private readonly ProductStockService _sut;

    public ProductStockServiceTests()
    {
        _sut = new ProductStockService(_repositoryMock.Object);
    }

    [Fact]
    public async Task IsStockAvailableAsync_ShouldReturnTrueWhenQuantityIsAvailable()
    {
        var productId = Guid.NewGuid();
        var product = new Product(productId, "Monitor", 150m, 10);
        _repositoryMock.Setup(repository => repository.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var result = await _sut.IsStockAvailableAsync(productId, 5, CancellationToken.None).ConfigureAwait(false);

        result.Should().BeTrue();
        _repositoryMock.Verify(repository => repository.GetByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task IsStockAvailableAsync_ShouldReturnFalseWhenQuantityIsNotAvailable()
    {
        var productId = Guid.NewGuid();
        var product = new Product(productId, "Keyboard", 50m, 2);
        _repositoryMock.Setup(repository => repository.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var result = await _sut.IsStockAvailableAsync(productId, 3, CancellationToken.None).ConfigureAwait(false);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsStockAvailableAsync_ShouldThrowNotFoundExceptionWhenProductIsMissing()
    {
        var productId = Guid.NewGuid();
        _repositoryMock.Setup(repository => repository.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var act = async () => await _sut.IsStockAvailableAsync(productId, 1, CancellationToken.None).ConfigureAwait(false);

        var expectedError = DomainErrors.Product.NotFound(productId);

        await act.Should().ThrowAsync<NotFoundException>()
            .Where(exception => exception.Code == expectedError.Code)
            .WithMessage(expectedError.Message);
    }

    [Fact]
    public async Task IsStockAvailableAsync_ShouldThrowValidationExceptionWhenQuantityIsNotPositive()
    {
        var productId = Guid.NewGuid();

        var act = async () => await _sut.IsStockAvailableAsync(productId, 0, CancellationToken.None).ConfigureAwait(false);

        var expectedError = DomainErrors.Product.StockQuantityToCheckMustBePositive;

        await act.Should().ThrowAsync<ValidationException>()
            .Where(exception => exception.Code == expectedError.Code)
            .WithMessage(expectedError.Message);

        _repositoryMock.Verify(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
