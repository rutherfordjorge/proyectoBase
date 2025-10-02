using System.Collections.Generic;
using System.Security.Claims;

namespace ProyectoBase.Application.Abstractions;

/// <summary>
/// Proporciona funcionalidad para crear tokens JSON Web para usuarios autenticados.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Genera un token de acceso JWT firmado utilizando los claims proporcionados.
    /// </summary>
    /// <param name="claims">Los claims que se incluirán en el token.</param>
    /// <returns>El token JWT serializado.</returns>
    string GenerateAccessToken(IEnumerable<Claim> claims);
}
