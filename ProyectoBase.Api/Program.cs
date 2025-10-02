using ProyectoBase.Api.Middleware;
using ProyectoBase.Api.Options;
using ProyectoBase.Application;
using ProyectoBase.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    app.UseSwaggerUI();
}

app.UseMiddleware<ValidationExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.Run();
