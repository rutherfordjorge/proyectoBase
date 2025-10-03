using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using ProyectoBase.Api.Application.Abstractions;
using ProyectoBase.Api.Application.DTOs;
using ProyectoBase.Api.Application.Mappings;
using ProyectoBase.Api.Application.Products.Commands.CreateProduct;
using ProyectoBase.Api.Application.Products.Commands.DeleteProduct;
using ProyectoBase.Api.Application.Products.Commands.UpdateProduct;
using ProyectoBase.Api.Application.Products.Common;
using ProyectoBase.Api.Application.Products.Queries.GetAllProducts;
using ProyectoBase.Api.Application.Products.Queries.GetProductById;
using ProyectoBase.Api.Domain.Entities;
using ProyectoBase.Api.Domain.Exceptions;
using Xunit;

namespace ProyectoBase.Api.Application.Tests.Products;

/// <summary>
/// Contiene pruebas unitarias para los handlers relacionados con productos.
/// </summary>
public class ProductHandlersTests
{
    private static IMapper CreateMapper()
    {
        var configuration = new MapperConfiguration(config =>
        {
            config.AddProfile<ProductProfile>();
        });

        return configuration.CreateMapper();
    }

    [Fact]
    public async Task CreateProductHandler_should_persist_product_and_clear_cache()
    {
        var repositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var cacheMock = new Mock<IDistributedCache>(MockBehavior.Strict);
        var loggerMock = new Mock<ILogger<CreateProductCommandHandler>>();
        var mapper = CreateMapper();
        var productDto = new ProductCreateDto
        {
            Name = "Nuevo producto",
            Description = "Descripción",
            Price = 100,
            Stock = 5,
        };

        repositoryMock
            .Setup(repository => repository.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        cacheMock
            .Setup(cache => cache.RemoveAsync(ProductCacheSettings.AllProductsCacheKey, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateProductCommandHandler(repositoryMock.Object, mapper, cacheMock.Object, loggerMock.Object);

        var result = await handler.Handle(new CreateProductCommand(productDto), CancellationToken.None);

        Assert.Equal(productDto.Name, result.Name);
        Assert.Equal(productDto.Price, result.Price);
        Assert.Equal(productDto.Stock, result.Stock);

        repositoryMock.Verify(repository => repository.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        cacheMock.Verify(cache => cache.RemoveAsync(ProductCacheSettings.AllProductsCacheKey, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
        cacheMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateProductHandler_should_update_existing_product()
    {
        var repositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var cacheMock = new Mock<IDistributedCache>(MockBehavior.Strict);
        var loggerMock = new Mock<ILogger<UpdateProductCommandHandler>>();
        var mapper = CreateMapper();

        var existingProduct = new Product(Guid.NewGuid(), "Original", 10, 1, "Desc");
        var dto = new ProductUpdateDto
        {
            Id = existingProduct.Id,
            Name = "Actualizado",
            Description = "Actualización",
            Price = 20,
            Stock = 3,
        };

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(existingProduct.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        repositoryMock
            .Setup(repository => repository.UpdateAsync(existingProduct, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        cacheMock
            .Setup(cache => cache.RemoveAsync(ProductCacheSettings.AllProductsCacheKey, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new UpdateProductCommandHandler(repositoryMock.Object, mapper, cacheMock.Object, loggerMock.Object);

        var result = await handler.Handle(new UpdateProductCommand(dto), CancellationToken.None);

        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.Description, result.Description);
        Assert.Equal(dto.Price, result.Price);
        Assert.Equal(dto.Stock, result.Stock);

        repositoryMock.Verify(repository => repository.GetByIdAsync(existingProduct.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(repository => repository.UpdateAsync(existingProduct, It.IsAny<CancellationToken>()), Times.Once);
        cacheMock.Verify(cache => cache.RemoveAsync(ProductCacheSettings.AllProductsCacheKey, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
        cacheMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateProductHandler_should_throw_when_product_not_found()
    {
        var repositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var cacheMock = new Mock<IDistributedCache>(MockBehavior.Strict);
        var loggerMock = new Mock<ILogger<UpdateProductCommandHandler>>();
        var mapper = CreateMapper();
        var dto = new ProductUpdateDto
        {
            Id = Guid.NewGuid(),
            Name = "Producto",
            Description = "Desc",
            Price = 30,
            Stock = 4,
        };

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(dto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var handler = new UpdateProductCommandHandler(repositoryMock.Object, mapper, cacheMock.Object, loggerMock.Object);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new UpdateProductCommand(dto), CancellationToken.None));

        repositoryMock.Verify(repository => repository.GetByIdAsync(dto.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
        cacheMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetAllProductsHandler_should_return_cached_items_when_available()
    {
        var repositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var cacheMock = new Mock<IDistributedCache>(MockBehavior.Strict);
        var loggerMock = new Mock<ILogger<GetAllProductsQueryHandler>>();
        var mapper = CreateMapper();

        var cachedProducts = new List<ProductResponseDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Producto en caché",
                Price = 50,
                Stock = 10,
            },
        };

        var cachedBytes = JsonSerializer.SerializeToUtf8Bytes(cachedProducts, ProductCacheSettings.SerializerOptions);

        cacheMock
            .Setup(cache => cache.GetAsync(ProductCacheSettings.AllProductsCacheKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedBytes);

        var handler = new GetAllProductsQueryHandler(repositoryMock.Object, mapper, cacheMock.Object, loggerMock.Object);

        var result = await handler.Handle(new GetAllProductsQuery(), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(cachedProducts[0].Name, result.First().Name);

        cacheMock.Verify(cache => cache.GetAsync(ProductCacheSettings.AllProductsCacheKey, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
        cacheMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetAllProductsHandler_should_fetch_and_cache_when_not_cached()
    {
        var repositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var cacheMock = new Mock<IDistributedCache>(MockBehavior.Strict);
        var loggerMock = new Mock<ILogger<GetAllProductsQueryHandler>>();
        var mapper = CreateMapper();

        var products = new List<Product>
        {
            new(Guid.NewGuid(), "Producto 1", 10, 1),
            new(Guid.NewGuid(), "Producto 2", 20, 2),
        };

        cacheMock
            .Setup(cache => cache.GetAsync(ProductCacheSettings.AllProductsCacheKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        repositoryMock
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products.AsReadOnly());

        cacheMock
            .Setup(cache => cache.SetAsync(
                ProductCacheSettings.AllProductsCacheKey,
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new GetAllProductsQueryHandler(repositoryMock.Object, mapper, cacheMock.Object, loggerMock.Object);

        var result = await handler.Handle(new GetAllProductsQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);

        cacheMock.Verify(cache => cache.GetAsync(ProductCacheSettings.AllProductsCacheKey, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        cacheMock.Verify(cache => cache.SetAsync(
            ProductCacheSettings.AllProductsCacheKey,
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
        cacheMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetProductByIdHandler_should_return_null_when_not_found()
    {
        var repositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var loggerMock = new Mock<ILogger<GetProductByIdQueryHandler>>();
        var mapper = CreateMapper();
        var id = Guid.NewGuid();

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var handler = new GetProductByIdQueryHandler(repositoryMock.Object, mapper, loggerMock.Object);

        var result = await handler.Handle(new GetProductByIdQuery(id), CancellationToken.None);

        Assert.Null(result);

        repositoryMock.Verify(repository => repository.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteProductHandler_should_remove_product_and_cache()
    {
        var repositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);
        var cacheMock = new Mock<IDistributedCache>(MockBehavior.Strict);
        var loggerMock = new Mock<ILogger<DeleteProductCommandHandler>>();
        var id = Guid.NewGuid();

        repositoryMock
            .Setup(repository => repository.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        cacheMock
            .Setup(cache => cache.RemoveAsync(ProductCacheSettings.AllProductsCacheKey, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new DeleteProductCommandHandler(repositoryMock.Object, cacheMock.Object, loggerMock.Object);

        await handler.Handle(new DeleteProductCommand(id), CancellationToken.None);

        repositoryMock.Verify(repository => repository.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        cacheMock.Verify(cache => cache.RemoveAsync(ProductCacheSettings.AllProductsCacheKey, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
        cacheMock.VerifyNoOtherCalls();
    }
}
