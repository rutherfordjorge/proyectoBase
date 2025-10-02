using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProyectoBase.Api.Application.Abstractions;
using ProyectoBase.Api.Application.Options;

namespace ProyectoBase.Api.Infrastructure.Authentication;

/// <summary>
/// Proporciona utilidades para generar tokens JSON Web para usuarios autenticados.
/// </summary>
public class TokenService : ITokenService
{
    private readonly JwtOptions _options;
    private readonly SigningCredentials _signingCredentials;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="TokenService"/>.
    /// </summary>
    /// <param name="options">Las opciones de configuración de JWT.</param>
    public TokenService(IOptions<JwtOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _options = options.Value;

        if (string.IsNullOrWhiteSpace(_options.Key))
        {
            throw new InvalidOperationException("Se requiere configurar la clave de firma para JWT.");
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        _signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
    }

    /// <inheritdoc />
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        ArgumentNullException.ThrowIfNull(claims);

        var claimsArray = claims as Claim[] ?? claims.ToArray();

        var jwtToken = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claimsArray,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_options.AccessTokenExpirationMinutes),
            signingCredentials: _signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
}
