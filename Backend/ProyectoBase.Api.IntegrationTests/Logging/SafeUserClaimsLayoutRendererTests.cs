using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using ProyectoBase.Api.Logging;
using Xunit;

namespace ProyectoBase.Api.IntegrationTests.Logging;

public sealed class SafeUserClaimsLayoutRendererTests
{
    [Fact]
    public void Render_ReturnsClaimTypeCounts()
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user-123"),
                new Claim(ClaimTypes.Email, "person@example.com"),
                new Claim(ClaimTypes.Role, "admin"),
                new Claim(ClaimTypes.Role, "auditor"),
            }, authenticationType: "Test")),
        };

        var claimsByType = SafeUserClaimsLayoutRenderer.GetClaimTypeCounts(httpContext);

        var serialized = JsonSerializer.Serialize(claimsByType);
        var json = JsonNode.Parse(serialized)!.AsObject();
        json[ClaimTypes.NameIdentifier]!.GetValue<int>().Should().Be(1);
        json[ClaimTypes.Email]!.GetValue<int>().Should().Be(1);
        json[ClaimTypes.Role]!.GetValue<int>().Should().Be(2);
        serialized.Should().NotContain("person@example.com", because: "emails must never be logged");
        serialized.Should().NotContain("user-123");
    }

    [Fact]
    public void Render_ReturnsEmptyWhenUserIsUnauthenticated()
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity()),
        };

        var claimsByType = SafeUserClaimsLayoutRenderer.GetClaimTypeCounts(httpContext);

        claimsByType.Should().BeEmpty();
    }
}
