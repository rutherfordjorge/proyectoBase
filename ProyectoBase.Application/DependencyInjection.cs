using System;
using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ProyectoBase.Application.Abstractions;
using ProyectoBase.Application.Services.Products;

namespace ProyectoBase.Application;

/// <summary>
/// Provides extension methods to register application layer services within the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers the application layer dependencies including services, validators and AutoMapper profiles.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The configured <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}
