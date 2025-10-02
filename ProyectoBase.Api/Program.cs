using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using NLog.Web;
using ProyectoBase.Api.Middlewares;
using ProyectoBase.Api.Options;
using ProyectoBase.Api.Swagger;
using ProyectoBase.Api.Swagger.Filters;
using ProyectoBase.Application;
using ProyectoBase.Infrastructure;
using ProyectoBase.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Host.UseNLog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
static IEnumerable<string> GetXmlDocumentationPaths()
{
    var assemblies = new[]
    {
        typeof(Program).Assembly,
        typeof(ProyectoBase.Application.DependencyInjection).Assembly,
        typeof(Product).Assembly,
        typeof(ProyectoBase.Infrastructure.DependencyInjection).Assembly,
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
        typeof(ProyectoBase.Application.DependencyInjection).Assembly,
        typeof(ProyectoBase.Infrastructure.DependencyInjection).Assembly);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",
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

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.Configure<RedisOptions>(builder.Configuration.GetSection(RedisOptions.SectionName));
builder.Services.Configure<ConnectionStringsOptions>(builder.Configuration.GetSection(ConnectionStringsOptions.SectionName));
builder.Services.PostConfigure<ConnectionStringsOptions>(options =>
{
    options.DefaultConnection = builder.Configuration.GetConnectionString(ConnectionStringsOptions.DefaultConnectionName)
        ?? options.DefaultConnection
        ?? string.Empty;
});

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

app.MapControllers();

app.Run();
