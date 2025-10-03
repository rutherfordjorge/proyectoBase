using System;
using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ProyectoBase.Api.Application;

/// <summary>
/// Proporciona métodos de extensión para registrar los servicios de la capa de aplicación en el contenedor de dependencias.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra las dependencias de la capa de aplicación, incluidos servicios, validadores y perfiles de AutoMapper.
    /// </summary>
    /// <param name="services">La colección de servicios que se configurará.</param>
    /// <returns>La instancia configurada de <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddAutoMapper(typeof(StartupAssemblyMarker).Assembly);

        return services;
    }
}
