using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;
using ProyectoBase.Application.Abstractions;
using ProyectoBase.Infrastructure.Authentication;
using ProyectoBase.Infrastructure.Persistence;
using ProyectoBase.Infrastructure.Persistence.Repositories;

namespace ProyectoBase.Infrastructure;

/// <summary>
/// Provides extension methods to register infrastructure services and dependencies.
/// </summary>
public static class DependencyInjection
{
    private const string DefaultConnectionName = "DefaultConnection";
    private const string RetryPolicyName = "DefaultRetryPolicy";
    private const string CircuitBreakerPolicyName = "DefaultCircuitBreakerPolicy";
    private const string RepositoryReadPolicyName = "ProductRepository.Read";
    private const string RepositoryWritePolicyName = "ProductRepository.Write";

    /// <summary>
    /// Registers the services required by the infrastructure layer.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration source.</param>
    /// <returns>The configured <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString = configuration.GetConnectionString(DefaultConnectionName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException($"Connection string '{DefaultConnectionName}' was not found.");
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
                    logger.LogWarning(exception, "Retry {RetryCount} executed after {Delay} seconds.", retryCount, timeSpan.TotalSeconds);
                });

            var circuitBreakerPolicy = Policy.Handle<Exception>().CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (exception, breakDelay) =>
                {
                    logger.LogError(exception, "Circuit opened for {Delay} seconds due to repeated failures.", breakDelay.TotalSeconds);
                },
                onReset: () => logger.LogInformation("Circuit closed: operations have stabilized."),
                onHalfOpen: () => logger.LogInformation("Circuit half-open: next call is a trial."));

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
