using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;
using ProyectoBase.Api.Application.Abstractions;
using ProyectoBase.Api.Infrastructure.Authentication;
using ProyectoBase.Api.Infrastructure.Persistence;
using ProyectoBase.Api.Infrastructure.Persistence.Repositories;

namespace ProyectoBase.Api.Infrastructure;

/// <summary>
/// Proporciona métodos de extensión para registrar los servicios y dependencias de la infraestructura.
/// </summary>
public static class DependencyInjection
{
    private const string DefaultConnectionName = "DefaultConnection";
    private const string RetryPolicyName = "DefaultRetryPolicy";
    private const string CircuitBreakerPolicyName = "DefaultCircuitBreakerPolicy";
    private const string RepositoryReadPolicyName = "ProductRepository.Read";
    private const string RepositoryWritePolicyName = "ProductRepository.Write";

    /// <summary>
    /// Registra los servicios necesarios para la capa de infraestructura.
    /// </summary>
    /// <param name="services">La colección de servicios que se configurará.</param>
    /// <param name="configuration">La fuente de configuración de la aplicación.</param>
    /// <returns>La instancia configurada de <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString = configuration.GetConnectionString(DefaultConnectionName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException($"No se encontró la cadena de conexión '{DefaultConnectionName}'.");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure();
            });
        });

        services.AddScoped<ProductRepository>();
        services.AddSingleton<ITokenService, TokenService>();

        services.AddMemoryCache();
        services.AddDistributedMemoryCache();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["Redis:ConnectionString"] ?? string.Empty;
            options.InstanceName = configuration["Redis:InstanceName"] ?? string.Empty;
        });

        services.AddSingleton<IPolicyRegistry<string>>(provider =>
        {
            var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger("ResiliencePolicies");

            var registry = new PolicyRegistry();

            var retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt - 1)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    logger.LogWarning(exception, "Reintento {RetryCount} ejecutado después de {Delay} segundos.", retryCount, timeSpan.TotalSeconds);
                });

            var circuitBreakerPolicy = Policy.Handle<Exception>().CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (exception, breakDelay) =>
                {
                    logger.LogError(exception, "Circuito abierto durante {Delay} segundos debido a fallas reiteradas.", breakDelay.TotalSeconds);
                },
                onReset: () => logger.LogInformation("Circuito cerrado: las operaciones se estabilizaron."),
                onHalfOpen: () => logger.LogInformation("Circuito medio abierto: la siguiente llamada es de prueba."));

            registry.Add(RetryPolicyName, retryPolicy);
            registry.Add(CircuitBreakerPolicyName, circuitBreakerPolicy);

            var repositoryPolicy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);

            registry.Add(RepositoryWritePolicyName, repositoryPolicy);
            registry.Add(RepositoryReadPolicyName, repositoryPolicy);

            return registry;
        });

        services.AddSingleton<IReadOnlyPolicyRegistry<string>>(provider => provider.GetRequiredService<IPolicyRegistry<string>>());

        services.AddScoped<IProductRepository>(provider =>
        {
            var repository = provider.GetRequiredService<ProductRepository>();
            var registry = provider.GetRequiredService<IReadOnlyPolicyRegistry<string>>();
            var writePolicy = registry.Get<IAsyncPolicy>(RepositoryWritePolicyName);
            var readPolicy = registry.Get<IAsyncPolicy>(RepositoryReadPolicyName);

            return new ResilientProductRepository(repository, writePolicy, readPolicy);
        });

        return services;
    }
}
