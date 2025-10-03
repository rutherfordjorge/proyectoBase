# Paquetes NuGet por proyecto

El siguiente inventario detalla los paquetes NuGet utilizados en cada proyecto del repositorio, junto con su versión, propósito y un ejemplo concreto de dónde se utilizan en el código. Mantener esta tabla actualizada facilita detectar impactos cuando se cambian dependencias.

## Backend/ProyectoBase.Api/ProyectoBase.Api.Api.csproj

| Paquete | Versión | Propósito | Ejemplo en el código |
| --- | --- | --- | --- |
| Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.8 | Habilita la autenticación basada en tokens JWT para la API. | Configurado en `Program.cs` al registrar `AddAuthentication(...).AddJwtBearer(...)` para validar tokens entrantes.【F:Backend/ProyectoBase.Api/Program.cs†L161-L180】 |
| Microsoft.AspNetCore.OpenApi | 8.0.8 | Expone metadatos OpenAPI para los endpoints de ASP.NET Core. | `Program.cs` invoca `AddEndpointsApiExplorer()` para generar descripciones de endpoints utilizadas por Swagger.【F:Backend/ProyectoBase.Api/Program.cs†L29-L30】 |
| Microsoft.AspNetCore.Mvc.Versioning | 5.1.0 | Gestiona versiones de la API y permite negociar la versión solicitada. | `Program.cs` configura `AddApiVersioning` con lectores de versión por query string, encabezado, tipo de medio y segmento de ruta.【F:Backend/ProyectoBase.Api/Program.cs†L90-L100】 |
| Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer | 5.1.0 | Expone la información de versiones para herramientas como Swagger. | `Program.cs` llama a `AddVersionedApiExplorer`, lo que luego consume `ConfigureSwaggerOptions` al generar documentos por versión.【F:Backend/ProyectoBase.Api/Program.cs†L102-L106】【F:Backend/ProyectoBase.Api/Swagger/ConfigureSwaggerOptions.cs†L11-L32】 |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.8 | Proveedor EF Core para SQL Server usado por la capa de infraestructura. | El servicio de infraestructura registra `UseSqlServer` al crear el `ApplicationDbContext` para acceso a datos.【F:Backend/ProyectoBase.Infrastructure/DependencyInjection.cs†L44-L50】 |
| Microsoft.EntityFrameworkCore.Tools | 8.0.8 | Herramientas de diseño EF Core para generar migraciones y actualizaciones. | Las clases de migración generadas (por ejemplo, `20241010120000_InitialCreate`) usan las API de `MigrationBuilder` producidas por estas herramientas.【F:Backend/ProyectoBase.Infrastructure/Persistence/Migrations/20241010120000_InitialCreate.cs†L9-L40】 |
| NLog.Web.AspNetCore | 5.3.10 | Integra NLog con ASP.NET Core para logging estructurado. | La aplicación habilita NLog con `builder.Host.UseNLog()` y define layout renderers personalizados para registrar claims y encabezados seguros.【F:Backend/ProyectoBase.Api/Program.cs†L26-L27】【F:Backend/ProyectoBase.Api/Logging/SafeUserClaimsLayoutRenderer.cs†L12-L38】 |
| Swashbuckle.AspNetCore | 6.6.2 | Genera la interfaz y documentos Swagger para la API. | `Program.cs` registra `AddSwaggerGen` y agrega definiciones de seguridad y filtros personalizados para documentar respuestas estándar.【F:Backend/ProyectoBase.Api/Program.cs†L48-L88】【F:Backend/ProyectoBase.Api/Swagger/Filters/DefaultResponsesOperationFilter.cs†L13-L50】 |
| StyleCop.Analyzers | 1.2.0-beta.507 | Aplica reglas de estilo y análisis estático durante la compilación. | Se incluye como analizador con `PrivateAssets="all"` en el archivo de proyecto para reforzar el estilo sin distribuirlo en los consumidores.【F:Backend/ProyectoBase.Api/ProyectoBase.Api.Api.csproj†L25-L28】 |

## Backend/ProyectoBase.Application/ProyectoBase.Api.Application.csproj

| Paquete | Versión | Propósito | Ejemplo en el código |
| --- | --- | --- | --- |
| FluentValidation | 11.9.0 | Define reglas de validación declarativa para DTOs. | `CreateProductDtoValidator` encadena reglas `RuleFor` para validar nombre, precio y stock antes de procesar comandos.【F:Backend/ProyectoBase.Application/Validators/CreateProductDtoValidator.cs†L9-L28】 |
| MediatR | 12.1.1 | Implementa el patrón mediator para comandos y consultas. | `CreateProductCommand` y su `CreateProductCommandHandler` usan `IRequest`/`IRequestHandler` para orquestar la creación de productos vía MediatR.【F:Backend/ProyectoBase.Application/Services/Products/CreateProductCommand.cs†L9-L24】【F:Backend/ProyectoBase.Application/Services/Products/CreateProductCommandHandler.cs†L12-L28】 |
| AutoMapper | 12.0.1 | Automatiza el mapeo entre entidades y DTOs. | `ProductProfile` define `CreateMap` para transformar `Product` y sus DTOs, incluyendo lógica adicional para sincronizar el stock.【F:Backend/ProyectoBase.Application/Mappings/ProductProfile.cs†L11-L43】 |
| AutoMapper.Extensions.Microsoft.DependencyInjection | 12.0.1 | Registra perfiles de AutoMapper en el contenedor de dependencias. | `AddApplication` invoca `services.AddAutoMapper(...)` para descubrir y registrar los perfiles declarados en la asamblea.【F:Backend/ProyectoBase.Application/DependencyInjection.cs†L25-L33】 |
| Microsoft.Extensions.Caching.Abstractions | 8.0.8 | Proporciona contratos de caché (memoria/distribuida) reutilizables. | `ProductService` interactúa con `IDistributedCache` para cachear resultados y reducir lecturas repetidas desde el repositorio.【F:Backend/ProyectoBase.Application/Services/Products/ProductService.cs†L26-L156】 |
| StyleCop.Analyzers | 1.2.0-beta.507 | Enforce estilo consistente durante la compilación del proyecto de aplicación. | Declarado como analizador en el archivo de proyecto para ejecutar reglas de StyleCop sin exponerlas a consumidores.【F:Backend/ProyectoBase.Application/ProyectoBase.Api.Application.csproj†L22-L25】 |

## Backend/ProyectoBase.Domain/ProyectoBase.Api.Domain.csproj

| Paquete | Versión | Propósito | Ejemplo en el código |
| --- | --- | --- | --- |
| StyleCop.Analyzers | 1.2.0-beta.507 | Controla el cumplimiento de convenciones de estilo en la capa de dominio. | El paquete se agrega como analizador en el proyecto de dominio para validar estilo y documentación durante la compilación.【F:Backend/ProyectoBase.Domain/ProyectoBase.Api.Domain.csproj†L13-L18】 |

## Backend/ProyectoBase.Infrastructure/ProyectoBase.Api.Infrastructure.csproj

| Paquete | Versión | Propósito | Ejemplo en el código |
| --- | --- | --- | --- |
| Microsoft.EntityFrameworkCore | 8.0.8 | Núcleo de EF Core para definir contextos y repositorios. | `ApplicationDbContext` hereda de `DbContext` y expone `DbSet<Product>` para acceder a las tablas mapeadas.【F:Backend/ProyectoBase.Infrastructure/Persistence/ApplicationDbContext.cs†L7-L35】 |
| Microsoft.EntityFrameworkCore.Design | 8.0.8 | Servicios de diseño EF Core para compatibilidad con herramientas y migraciones. | Las migraciones incluidas en `Persistence/Migrations` dependen de los servicios de diseño para crear y actualizar el esquema durante `dotnet ef` o pruebas.【F:Backend/ProyectoBase.Infrastructure/ProyectoBase.Api.Infrastructure.csproj†L15-L18】【F:Backend/ProyectoBase.Infrastructure/Persistence/Migrations/20241010120000_InitialCreate.cs†L15-L41】 |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.8 | Proveedor EF Core para bases de datos SQL Server. | Registrado en `AddInfrastructure` mediante `options.UseSqlServer` al construir el contexto de datos de la aplicación.【F:Backend/ProyectoBase.Infrastructure/DependencyInjection.cs†L44-L50】 |
| Microsoft.Extensions.Caching.StackExchangeRedis | 8.0.8 | Implementa caché distribuida respaldada por Redis. | La configuración de infraestructura llama a `AddStackExchangeRedisCache` para almacenar colecciones en caché compartida.【F:Backend/ProyectoBase.Infrastructure/DependencyInjection.cs†L55-L61】 |
| Polly | 7.2.4 | Provee políticas de resiliencia (reintentos, circuit breakers). | `AddInfrastructure` define políticas `WaitAndRetryAsync` y `CircuitBreakerAsync`, que se combinan en `ResilientProductRepository` al ejecutar operaciones de datos.【F:Backend/ProyectoBase.Infrastructure/DependencyInjection.cs†L63-L108】【F:Backend/ProyectoBase.Infrastructure/Persistence/Repositories/ResilientProductRepository.cs†L11-L97】 |
| System.IdentityModel.Tokens.Jwt | 7.6.0 | Genera y manipula tokens JWT firmados. | `TokenService` crea instancias de `JwtSecurityToken` y las serializa con `JwtSecurityTokenHandler` para emitir tokens de acceso.【F:Backend/ProyectoBase.Infrastructure/Authentication/TokenService.cs†L17-L55】 |
| StyleCop.Analyzers | 1.2.0-beta.507 | Ejecuta reglas de estilo en la capa de infraestructura. | Incluido como analizador en el archivo de proyecto para reforzar la calidad del código.【F:Backend/ProyectoBase.Infrastructure/ProyectoBase.Api.Infrastructure.csproj†L23-L26】 |

## Backend/ProyectoBase.Application.Tests/ProyectoBase.Api.Application.Tests.csproj

| Paquete | Versión | Propósito | Ejemplo en el código |
| --- | --- | --- | --- |
| FluentValidation | 11.9.0 | Expone los validadores y extensiones de prueba (`FluentValidation.TestHelper`) usados por los tests. | `CreateProductDtoValidatorTests` llama a `validator.TestValidate(...)` y usa extensiones como `ShouldHaveValidationErrorFor`.【F:Backend/ProyectoBase.Application.Tests/Validators/CreateProductDtoValidatorTests.cs†L15-L58】 |
| Microsoft.NET.Test.Sdk | 17.11.1 | Proporciona la infraestructura para descubrir y ejecutar pruebas .NET. | Permite que los atributos `[Fact]` de xUnit en los proyectos de prueba se ejecuten mediante el runner de .NET.【F:Backend/ProyectoBase.Application.Tests/Validators/CreateProductDtoValidatorTests.cs†L15-L58】 |
| xunit | 2.9.0 | Framework de pruebas unitarias utilizado en el proyecto. | Las pruebas marcan métodos con `[Fact]` para validar casos de negocio del validador y servicios de dominio.【F:Backend/ProyectoBase.Application.Tests/Validators/CreateProductDtoValidatorTests.cs†L15-L58】 |
| xunit.runner.visualstudio | 2.8.2 | Integra las pruebas xUnit con Visual Studio y otros runners basados en VS Test. | Declarado con `PrivateAssets="all"` para habilitar la ejecución desde `dotnet test` y herramientas de CI.【F:Backend/ProyectoBase.Application.Tests/ProyectoBase.Api.Application.Tests.csproj†L17-L20】 |
| coverlet.collector | 6.0.4 | Recolecta métricas de cobertura de código durante la ejecución de pruebas. | Registrado en el archivo de proyecto para que `dotnet test` capture cobertura sin configuración adicional.【F:Backend/ProyectoBase.Application.Tests/ProyectoBase.Api.Application.Tests.csproj†L21-L24】 |
| StyleCop.Analyzers | 1.2.0-beta.507 | Enforce estilo y convenciones en el código de pruebas. | Incluido como analizador con `PrivateAssets="all"` para revisar las pruebas sin afectar a consumidores externos.【F:Backend/ProyectoBase.Application.Tests/ProyectoBase.Api.Application.Tests.csproj†L25-L28】 |

## Backend/ProyectoBase.Domain.Tests/ProyectoBase.Api.Domain.Tests.csproj

| Paquete | Versión | Propósito | Ejemplo en el código |
| --- | --- | --- | --- |
| FluentAssertions | 6.12.0 | Proporciona aserciones fluidas y expresivas para pruebas. | `ProductTests` usa `product.Id.Should().Be(...)` y `Should().Throw<ValidationException>()` para verificar comportamientos de dominio.【F:Backend/ProyectoBase.Domain.Tests/Entities/ProductTests.cs†L10-L44】 |
| Microsoft.NET.Test.Sdk | 17.11.1 | Infraestructura de ejecución de pruebas para xUnit. | Permite ejecutar los `[Fact]` y `[Theory]` definidos en las pruebas de dominio.【F:Backend/ProyectoBase.Domain.Tests/Entities/ProductTests.cs†L10-L44】 |
| Moq | 4.20.72 | Facilita la creación de mocks para dependencias en pruebas. | `ProductStockServiceTests` crea `Mock<IProductReadRepository>` y configura expectativas para validar la lógica de stock.【F:Backend/ProyectoBase.Domain.Tests/Services/ProductStockServiceTests.cs†L16-L44】 |
| xunit | 2.9.0 | Framework de pruebas empleado en la capa de dominio. | Atributos `[Fact]` y `[Theory]` definen los casos verificados en `ProductTests` y `ProductStockServiceTests`.【F:Backend/ProyectoBase.Domain.Tests/Entities/ProductTests.cs†L10-L44】【F:Backend/ProyectoBase.Domain.Tests/Services/ProductStockServiceTests.cs†L24-L58】 |
| xunit.runner.visualstudio | 2.8.2 | Runner compatible con Visual Studio para ejecutar pruebas xUnit. | Configurado con `PrivateAssets` en el archivo de proyecto para habilitar la ejecución desde herramientas compatibles con VS Test.【F:Backend/ProyectoBase.Domain.Tests/ProyectoBase.Api.Domain.Tests.csproj†L14-L18】 |
| coverlet.collector | 6.0.4 | Recolecta cobertura de código durante las pruebas de dominio. | Registrado en el archivo de proyecto para habilitar la recopilación automática de cobertura.【F:Backend/ProyectoBase.Domain.Tests/ProyectoBase.Api.Domain.Tests.csproj†L19-L22】 |
| StyleCop.Analyzers | 1.2.0-beta.507 | Analiza el estilo del código en los proyectos de prueba de dominio. | Declarado como analizador con `PrivateAssets="all"` en el archivo de proyecto.【F:Backend/ProyectoBase.Domain.Tests/ProyectoBase.Api.Domain.Tests.csproj†L23-L26】 |

## Backend/ProyectoBase.Api.IntegrationTests/ProyectoBase.Api.IntegrationTests.csproj

| Paquete | Versión | Propósito | Ejemplo en el código |
| --- | --- | --- | --- |
| FluentAssertions | 6.12.0 | Simplifica las aserciones en pruebas de integración. | `ProductsControllerTests` usa `Should().Be(...)` y `Should().ContainSingle()` para validar respuestas HTTP y payloads.【F:Backend/ProyectoBase.Api.IntegrationTests/Controllers/V1/ProductsControllerTests.cs†L26-L64】 |
| Microsoft.AspNetCore.Mvc.Testing | 8.0.8 | Proporciona `WebApplicationFactory` para hospedar la API en pruebas. | `CustomWebApplicationFactory` hereda de `WebApplicationFactory<Program>` para inicializar la aplicación con configuración de prueba.【F:Backend/ProyectoBase.Api.IntegrationTests/Infrastructure/CustomWebApplicationFactory.cs†L19-L79】 |
| Microsoft.Data.Sqlite | 8.0.8 | Ofrece un proveedor SQLite en memoria usado durante pruebas. | La factoría de pruebas crea `new SqliteConnection("DataSource=:memory:")` para una base efímera.【F:Backend/ProyectoBase.Api.IntegrationTests/Infrastructure/CustomWebApplicationFactory.cs†L57-L65】 |
| Microsoft.EntityFrameworkCore.Sqlite | 8.0.8 | Permite que EF Core opere contra SQLite durante las pruebas. | `CustomWebApplicationFactory` registra `options.UseSqlite(connection)` al crear el contexto de datos de prueba.【F:Backend/ProyectoBase.Api.IntegrationTests/Infrastructure/CustomWebApplicationFactory.cs†L62-L77】 |
| Microsoft.NET.Test.Sdk | 17.11.1 | Plataforma de ejecución de pruebas para xUnit en integración. | Activa la ejecución de los `[Fact]` definidos en `ProductsControllerTests` mediante el runner de .NET.【F:Backend/ProyectoBase.Api.IntegrationTests/Controllers/V1/ProductsControllerTests.cs†L26-L64】 |
| xunit | 2.9.0 | Framework de pruebas utilizado para validar escenarios end-to-end. | `ProductsControllerTests` marca métodos con `[Fact]` para los distintos casos de integración.【F:Backend/ProyectoBase.Api.IntegrationTests/Controllers/V1/ProductsControllerTests.cs†L26-L64】 |
| xunit.runner.visualstudio | 2.8.2 | Runner VS Test para las pruebas de integración. | Configurado con `PrivateAssets="all"` en el proyecto para que las pruebas se ejecuten desde `dotnet test` y pipelines CI.【F:Backend/ProyectoBase.Api.IntegrationTests/ProyectoBase.Api.IntegrationTests.csproj†L16-L20】 |
| coverlet.collector | 6.0.4 | Genera métricas de cobertura durante las pruebas de integración. | Registrado como colector de cobertura en el archivo de proyecto.【F:Backend/ProyectoBase.Api.IntegrationTests/ProyectoBase.Api.IntegrationTests.csproj†L21-L24】 |
| StyleCop.Analyzers | 1.2.0-beta.507 | Aplica reglas de estilo a los archivos de pruebas de integración. | Incluido como analizador con `PrivateAssets="all"` en el archivo de proyecto.【F:Backend/ProyectoBase.Api.IntegrationTests/ProyectoBase.Api.IntegrationTests.csproj†L25-L28】 |

> **Nota:** Cada vez que se agreguen, actualicen o eliminen paquetes NuGet, actualizar esta tabla y las referencias de ejemplo correspondientes garantiza que el inventario se mantenga útil y vigente.

