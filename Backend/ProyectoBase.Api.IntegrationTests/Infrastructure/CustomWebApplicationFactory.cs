using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ProyectoBase.Api.Api;
using ProyectoBase.Api.Domain.Entities;
using ProyectoBase.Api.Infrastructure.Persistence;

namespace ProyectoBase.Api.IntegrationTests.Infrastructure;

/// <summary>
/// Customizes the application factory to use an in-memory SQLite database for integration tests.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string JwtKey = "0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF";
    private const string JwtIssuer = "IntegrationTests";
    private const string JwtAudience = "IntegrationTests";
    private const string AdminRole = "Admin";

    /// <summary>
    /// Gets the identifier of the product inserted during database seeding.
    /// </summary>
    public Guid SeededProductId { get; private set; }

    /// <summary>
    /// Gets the JWT access token used to authenticate as an administrator in tests.
    /// </summary>
    public string AdminAccessToken { get; private set; } = string.Empty;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["Jwt:Key"] = JwtKey,
                ["Jwt:Issuer"] = JwtIssuer,
                ["Jwt:Audience"] = JwtAudience,
                ["Jwt:AccessTokenExpirationMinutes"] = "60",
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

            var distributedCacheDescriptor = services.SingleOrDefault(service => service.ServiceType == typeof(IDistributedCache));
            if (distributedCacheDescriptor is not null)
            {
                services.Remove(distributedCacheDescriptor);
            }

            services.AddDistributedMemoryCache();

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

            AdminAccessToken = GenerateJwtToken(AdminRole);
        });
    }

    /// <summary>
    /// Creates an HTTP client pre-configured with an administrator bearer token.
    /// </summary>
    /// <returns>An authenticated <see cref="HttpClient"/> instance.</returns>
    public HttpClient CreateAdminClient()
    {
        var client = CreateClient();

        if (string.IsNullOrWhiteSpace(AdminAccessToken))
        {
            AdminAccessToken = GenerateJwtToken(AdminRole);
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AdminAccessToken);

        return client;
    }

    private void SeedDatabase(ApplicationDbContext context)
    {
        SeededProductId = Guid.NewGuid();

        context.Products.RemoveRange(context.Products);

        var product = new Product(SeededProductId, "Integration Product", 99.90m, 15, "Product used for integration tests.");

        context.Products.Add(product);
        context.SaveChanges();
    }

    private string GenerateJwtToken(string role)
    {
        var now = DateTime.UtcNow;
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, $"Integration {role}"),
            new(ClaimTypes.Role, role),
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: JwtIssuer,
            audience: JwtAudience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(60),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
