using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using ProyectoBase.Api.Api.Middlewares;
using ProyectoBase.Api.Api.Options;
using ProyectoBase.Api.Api.Swagger;
using ProyectoBase.Api.Api.Swagger.Filters;
using ProyectoBase.Api.Application;
using ProyectoBase.Api.Application.Options;
using ProyectoBase.Api.Infrastructure;
using ProyectoBase.Api.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Host.UseNLog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();
static IEnumerable<string> GetXmlDocumentationPaths()
{
    var assemblies = new[]
    {
        typeof(Program).Assembly,
        typeof(ProyectoBase.Api.Application.DependencyInjection).Assembly,
        typeof(Product).Assembly,
        typeof(ProyectoBase.Api.Infrastructure.DependencyInjection).Assembly,
    };

    return assemblies
        .Select(assembly => Path.Combine(AppContext.BaseDirectory, $"{assembly.GetName().Name}.xml"))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .Where(File.Exists);
}

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options =>
{
    foreach (var xmlPath in GetXmlDocumentationPaths())
    {
        options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }

    options.OperationFilter<DefaultResponsesOperationFilter>();

    options.MapCodeEnumsFromAssemblies(
        typeof(Program).Assembly,
        typeof(Product).Assembly,
        typeof(ProyectoBase.Api.Application.DependencyInjection).Assembly,
        typeof(ProyectoBase.Api.Infrastructure.DependencyInjection).Assembly);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Encabezado de autorización JWT usando el esquema Bearer.",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
            },
            Array.Empty<string>()
        },
    });
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("x-api-version"),
        new MediaTypeApiVersionReader("x-api-version"),
        new UrlSegmentApiVersionReader());
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

var jwtSettings = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();

if (string.IsNullOrWhiteSpace(jwtSettings.Key))
{
    throw new InvalidOperationException("La clave de firma JWT no está configurada.");
}

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? Array.Empty<string>();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.Configure<RedisOptions>(builder.Configuration.GetSection(RedisOptions.SectionName));
builder.Services.Configure<ConnectionStringsOptions>(builder.Configuration.GetSection(ConnectionStringsOptions.SectionName));
builder.Services.PostConfigure<ConnectionStringsOptions>(options =>
{
    options.DefaultConnection = builder.Configuration.GetConnectionString(ConnectionStringsOptions.DefaultConnectionName)
        ?? options.DefaultConnection
        ?? string.Empty;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
    {
        if (allowedOrigins.Length == 0)
        {
            if (builder.Environment.IsProduction())
            {
                throw new InvalidOperationException("La configuración Cors:AllowedOrigins es obligatoria en producción.");
            }

            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
            return;
        }

        policy.WithOrigins(allowedOrigins);

        if (builder.Environment.IsProduction())
        {
            policy.WithMethods("GET", "POST", "PUT", "DELETE")
                .WithHeaders("content-type", "authorization");
        }
        else
        {
            policy.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    });
});

var healthChecksBuilder = builder.Services.AddHealthChecks();

var defaultConnection = builder.Configuration.GetConnectionString(ConnectionStringsOptions.DefaultConnectionName)
    ?? builder.Configuration[$"{ConnectionStringsOptions.SectionName}:{nameof(ConnectionStringsOptions.DefaultConnection)}"];

if (!string.IsNullOrWhiteSpace(defaultConnection))
{
    healthChecksBuilder.AddSqlServer(
        connectionString: defaultConnection,
        healthQuery: "SELECT 1;",
        name: "sqlserver",
        tags: new[] { "ready" });
}

var redisOptions = builder.Configuration.GetSection(RedisOptions.SectionName).Get<RedisOptions>();

if (!string.IsNullOrWhiteSpace(redisOptions?.ConnectionString))
{
    healthChecksBuilder.AddRedis(
        redisConnectionString: redisOptions.ConnectionString,
        name: "redis",
        tags: new[] { "ready" });
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
        ClockSkew = TimeSpan.Zero,
    };
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    var apiVersionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("DefaultCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => false,
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false,
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("ready", StringComparer.OrdinalIgnoreCase),
});

app.Run();
