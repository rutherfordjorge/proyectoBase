using System.Collections.Generic;
using System.Security.Claims;

namespace ProyectoBase.Application.Abstractions;

/// <summary>
/// Provides functionality to create JSON Web Tokens for authenticated users.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a signed JWT access token using the provided claims.
    /// </summary>
    /// <param name="claims">The claims to embed in the token.</param>
    /// <returns>The serialized JWT token.</returns>
    string GenerateAccessToken(IEnumerable<Claim> claims);
}
