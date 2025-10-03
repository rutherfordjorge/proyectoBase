using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using ProyectoBase.Api.IntegrationTests.Infrastructure;
using Xunit;

namespace ProyectoBase.Api.IntegrationTests.HealthChecks;

public class HealthChecksTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public HealthChecksTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetHealth_ShouldReturnOk()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
