using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProyectoBase.Api.Api;
using ProyectoBase.Api.Domain.Entities;
using ProyectoBase.Api.Infrastructure.Persistence;

namespace ProyectoBase.Api.IntegrationTests.Infrastructure;

/// <summary>
/// Customizes the application factory to use an in-memory SQLite database for integration tests.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// Gets the identifier of the product inserted during database seeding.
    /// </summary>
    public Guid SeededProductId { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF",
                ["Jwt:Issuer"] = "IntegrationTests",
                ["Jwt:Audience"] = "IntegrationTests",
                ["ConnectionStrings:DefaultConnection"] = "Server=(localdb)\\mssqllocaldb;Database=IntegrationTests;Trusted_Connection=True;",
                ["Redis:ConnectionString"] = "localhost",
                ["Redis:InstanceName"] = "IntegrationTests",
            };

            configurationBuilder.AddInMemoryCollection(settings);
        });

        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (dbContextDescriptor is not null)
            {
                services.Remove(dbContextDescriptor);
            }

            var applicationDbContextDescriptor = services.SingleOrDefault(service => service.ServiceType == typeof(ApplicationDbContext));
            if (applicationDbContextDescriptor is not null)
            {
                services.Remove(applicationDbContextDescriptor);
            }

            var sqliteConnection = new SqliteConnection("DataSource=:memory:");
            sqliteConnection.Open();

            services.AddSingleton(sqliteConnection);

            services.AddDbContext<ApplicationDbContext>((provider, options) =>
            {
                var connection = provider.GetRequiredService<SqliteConnection>();
                options.UseSqlite(connection);
            });

            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<ApplicationDbContext>();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            SeedDatabase(context);
        });
    }

    private void SeedDatabase(ApplicationDbContext context)
    {
        SeededProductId = Guid.NewGuid();

        context.Products.RemoveRange(context.Products);

        var product = new Product(SeededProductId, "Integration Product", 99.90m, 15, "Product used for integration tests.");

        context.Products.Add(product);
        context.SaveChanges();
    }
}
