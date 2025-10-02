using System.Text.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using ProyectoBase.Api.Api.Logging;
using Xunit;

namespace ProyectoBase.Api.IntegrationTests.Logging;

public sealed class SafeRequestHeadersLayoutRendererTests
{
    [Fact]
    public void Render_MasksSensitiveHeaders()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Add("Authorization", new StringValues("Bearer super-secret-token"));
        httpContext.Request.Headers.Add("X-Api-Key", new StringValues("apikey-123"));
        httpContext.Request.Headers.Add("X-Correlation-Id", new StringValues("correlation"));

        var sanitized = SafeRequestHeadersLayoutRenderer.SanitizeHeaders(httpContext);

        var serialized = JsonSerializer.Serialize(sanitized);
        var json = JsonNode.Parse(serialized)!.AsObject();
        json["Authorization"]!.GetValue<string>().Should().Be("***");
        json["X-Api-Key"]!.GetValue<string>().Should().Be("***");
        json["X-Correlation-Id"]!.GetValue<string>().Should().Be("correlation");
        serialized.Should().NotContain("super-secret-token", because: "tokens must never be logged");
    }

    [Fact]
    public void Render_MasksSensitiveValuesEvenForNonSensitiveHeaders()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Add("X-Custom", new StringValues("password=PlainText"));

        var sanitized = SafeRequestHeadersLayoutRenderer.SanitizeHeaders(httpContext);

        var serialized = JsonSerializer.Serialize(sanitized);
        var json = JsonNode.Parse(serialized)!.AsObject();
        json["X-Custom"]!.GetValue<string>().Should().Be("***");
    }
}
