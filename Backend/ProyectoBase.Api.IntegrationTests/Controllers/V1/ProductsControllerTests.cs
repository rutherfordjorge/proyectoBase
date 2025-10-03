using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using ProyectoBase.Api.IntegrationTests.Infrastructure;
using ProyectoBase.Api.Application.DTOs;
using Xunit;

namespace ProyectoBase.Api.IntegrationTests.Controllers.V1;

public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

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

        var products = await response.Content.ReadFromJsonAsync<List<ProductResponseDto>>(JsonOptions).ConfigureAwait(false);

        products.Should().NotBeNull();
        products!.Should().Contain(product => product.Id == _factory.SeededProductId);

        var product = products.Single(item => item.Id == _factory.SeededProductId);
        product.Id.Should().Be(_factory.SeededProductId);
        product.Name.Should().Be("Integration Product");
        product.Price.Should().Be(99.90m);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnExpectedProduct()
    {
        var response = await _client.GetAsync($"/api/v1/Products/{_factory.SeededProductId}").ConfigureAwait(false);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var product = await response.Content.ReadFromJsonAsync<ProductResponseDto>(JsonOptions).ConfigureAwait(false);

        product.Should().NotBeNull();
        product!.Id.Should().Be(_factory.SeededProductId);
        product.Name.Should().Be("Integration Product");
    }

    [Fact]
    public async Task GetProductById_ShouldReturnNotFoundForUnknownProduct()
    {
        var response = await _client.GetAsync($"/api/v1/Products/{Guid.NewGuid()}").ConfigureAwait(false);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var error = await ReadStandardErrorAsync(response).ConfigureAwait(false);

        error.Should().NotBeNull();
        error!.Status.Should().Be((int)HttpStatusCode.NotFound);
        error.Error.Message.Should().Be("Recurso no encontrado");
        error.Details.ValueKind.Should().Be(JsonValueKind.String);
        error.Details.GetString().Should().Contain("El producto solicitado no fue encontrado.");
    }

    [Fact]
    public async Task PostProducts_ShouldCreateProductSuccessfully()
    {
        using var adminClient = _factory.CreateAdminClient();

        var request = new ProductCreateDto
        {
            Name = "Integration Product Created",
            Description = "Product created during integration testing.",
            Price = 45.75m,
            Stock = 20,
        };

        Guid createdProductId = Guid.Empty;

        try
        {
            var response = await adminClient.PostAsJsonAsync("/api/v1/Products", request).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var createdProduct = await response.Content
                .ReadFromJsonAsync<ProductResponseDto>(JsonOptions)
                .ConfigureAwait(false);

            createdProduct.Should().NotBeNull();
            createdProduct!.Id.Should().NotBeEmpty();
            createdProduct.Name.Should().Be(request.Name);
            createdProduct.Description.Should().Be(request.Description);
            createdProduct.Price.Should().Be(request.Price);
            createdProduct.Stock.Should().Be(request.Stock);

            createdProductId = createdProduct.Id;

            response.Headers.Location.Should().NotBeNull();
            response.Headers.Location!.OriginalString.Should().Contain(createdProductId.ToString());
        }
        finally
        {
            if (createdProductId != Guid.Empty)
            {
                await adminClient.DeleteAsync($"/api/v1/Products/{createdProductId}").ConfigureAwait(false);
            }
        }
    }

    [Fact]
    public async Task PostProducts_ShouldReturnBadRequest_WhenValidationFails()
    {
        using var adminClient = _factory.CreateAdminClient();

        var invalidRequest = new ProductCreateDto
        {
            Name = string.Empty,
            Description = new string('x', 10),
            Price = 0m,
            Stock = -5,
        };

        var response = await adminClient.PostAsJsonAsync("/api/v1/Products", invalidRequest).ConfigureAwait(false);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await ReadStandardErrorAsync(response).ConfigureAwait(false);

        error.Should().NotBeNull();
        error!.Status.Should().Be((int)HttpStatusCode.BadRequest);
        error.Error.Message.Should().Be("Solicitud inválida");
        error.Details.ValueKind.Should().Be(JsonValueKind.Array);

        var messages = error.Details
            .EnumerateArray()
            .Select(element => element.TryGetProperty("message", out var message) ? message.GetString() : null)
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .ToArray();

        messages.Should().Contain("El nombre del producto es obligatorio.");
        messages.Should().Contain("El precio debe ser mayor que cero.");
        messages.Should().Contain("El inventario debe ser un número mayor o igual que cero.");
    }

    [Fact]
    public async Task PostProducts_ShouldReturnBadRequest_WhenDomainValidationFails()
    {
        using var adminClient = _factory.CreateAdminClient();

        var invalidRequest = new ProductCreateDto
        {
            Name = "A ",
            Description = "Nombre con espacios finales provoca validación de dominio.",
            Price = 10m,
            Stock = 3,
        };

        var response = await adminClient.PostAsJsonAsync("/api/v1/Products", invalidRequest).ConfigureAwait(false);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await ReadStandardErrorAsync(response).ConfigureAwait(false);

        error.Should().NotBeNull();
        error!.Status.Should().Be((int)HttpStatusCode.BadRequest);
        error.Error.Message.Should().Be("Solicitud inválida");
        error.Details.ValueKind.Should().Be(JsonValueKind.Array);

        var detail = error.Details
            .EnumerateArray()
            .First();

        detail.TryGetProperty("message", out var detailMessage).Should().BeTrue();
        detailMessage.GetString().Should().Contain("El nombre del producto debe tener entre 2 y 100 caracteres.");
    }

    [Fact]
    public async Task PutProducts_ShouldUpdateProductSuccessfully()
    {
        using var adminClient = _factory.CreateAdminClient();

        var productToUpdate = await CreateProductAsync(adminClient, new ProductCreateDto
        {
            Name = "Product To Update",
            Description = "Original description",
            Price = 15.5m,
            Stock = 8,
        }).ConfigureAwait(false);

        try
        {
            var updateRequest = new ProductUpdateDto
            {
                Id = productToUpdate.Id,
                Name = "Updated Product",
                Description = "Updated description",
                Price = 25.5m,
                Stock = 12,
            };

            var response = await adminClient
                .PutAsJsonAsync($"/api/v1/Products/{productToUpdate.Id}", updateRequest)
                .ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var updatedProduct = await response.Content
                .ReadFromJsonAsync<ProductResponseDto>(JsonOptions)
                .ConfigureAwait(false);

            updatedProduct.Should().NotBeNull();
            updatedProduct!.Id.Should().Be(productToUpdate.Id);
            updatedProduct.Name.Should().Be(updateRequest.Name);
            updatedProduct.Description.Should().Be(updateRequest.Description);
            updatedProduct.Price.Should().Be(updateRequest.Price);
            updatedProduct.Stock.Should().Be(updateRequest.Stock);
        }
        finally
        {
            await adminClient.DeleteAsync($"/api/v1/Products/{productToUpdate.Id}").ConfigureAwait(false);
        }
    }

    [Fact]
    public async Task DeleteProducts_ShouldRemoveProductSuccessfully()
    {
        using var adminClient = _factory.CreateAdminClient();

        var productToDelete = await CreateProductAsync(adminClient, new ProductCreateDto
        {
            Name = "Product To Delete",
            Description = "Temporary product for deletion",
            Price = 12m,
            Stock = 2,
        }).ConfigureAwait(false);

        var response = await adminClient
            .DeleteAsync($"/api/v1/Products/{productToDelete.Id}")
            .ConfigureAwait(false);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client
            .GetAsync($"/api/v1/Products/{productToDelete.Id}")
            .ConfigureAwait(false);

        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteProducts_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        using var adminClient = _factory.CreateAdminClient();

        var response = await adminClient
            .DeleteAsync($"/api/v1/Products/{Guid.NewGuid()}")
            .ConfigureAwait(false);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var error = await ReadStandardErrorAsync(response).ConfigureAwait(false);

        error.Should().NotBeNull();
        error!.Status.Should().Be((int)HttpStatusCode.NotFound);
        error.Error.Message.Should().Be("Recurso no encontrado");
        error.Details.ValueKind.Should().Be(JsonValueKind.String);
        error.Details.GetString().Should().Contain("El producto solicitado no fue encontrado.");
    }

    private static async Task<ProductResponseDto> CreateProductAsync(HttpClient client, ProductCreateDto request)
    {
        var response = await client.PostAsJsonAsync("/api/v1/Products", request).ConfigureAwait(false);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdProduct = await response.Content
            .ReadFromJsonAsync<ProductResponseDto>(JsonOptions)
            .ConfigureAwait(false);

        createdProduct.Should().NotBeNull();

        return createdProduct!;
    }

    private static async Task<StandardErrorResponse?> ReadStandardErrorAsync(HttpResponseMessage response)
    {
        using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

        return await JsonSerializer.DeserializeAsync<StandardErrorResponse>(stream, JsonOptions).ConfigureAwait(false);
    }

    private sealed record StandardErrorResponse(string TraceId, int Status, ErrorContent Error, JsonElement Details);

    private sealed record ErrorContent(string Code, string Message);
}
