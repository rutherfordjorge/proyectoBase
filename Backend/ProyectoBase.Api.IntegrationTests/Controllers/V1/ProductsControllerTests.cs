using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using ProyectoBase.Api.IntegrationTests.Infrastructure;
using ProyectoBase.Api.Application.DTOs;
using Xunit;

namespace ProyectoBase.Api.IntegrationTests.Controllers.V1;

public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ProductsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ShouldReturnSeededProduct()
    {
        var response = await _client.GetAsync("/api/v1/Products").ConfigureAwait(false);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var products = await response.Content.ReadFromJsonAsync<List<ProductResponseDto>>().ConfigureAwait(false);

        products.Should().NotBeNull();
        products!.Should().ContainSingle();

        var product = products.Single();
        product.Id.Should().Be(_factory.SeededProductId);
        product.Name.Should().Be("Integration Product");
        product.Price.Should().Be(99.90m);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnExpectedProduct()
    {
        var response = await _client.GetAsync($"/api/v1/Products/{_factory.SeededProductId}").ConfigureAwait(false);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var product = await response.Content.ReadFromJsonAsync<ProductResponseDto>().ConfigureAwait(false);

        product.Should().NotBeNull();
        product!.Id.Should().Be(_factory.SeededProductId);
        product.Name.Should().Be("Integration Product");
    }

    [Fact]
    public async Task GetProductById_ShouldReturnNotFoundForUnknownProduct()
    {
        var response = await _client.GetAsync($"/api/v1/Products/{Guid.NewGuid()}").ConfigureAwait(false);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
