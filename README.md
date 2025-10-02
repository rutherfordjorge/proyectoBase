# ProyectoBase

Aplicación **fullstack** desarrollada con **ASP.NET Core 8 WebAPI** y **Angular 14**.  
Este repositorio sirve como plantilla base para construir aplicaciones modernas con separación clara de backend y frontend.

---

## 🚀 Tecnologías

- **Backend:** .NET 8 (ASP.NET Core WebAPI)
- **Frontend:** Angular 14
- **Comunicación:** REST API con CORS configurable
- **Arquitectura inicial:** CRUD completo de ejemplo (`Products`)

```bash
ProyectoBase/
│
├── ProyectoBase.Api        # API en ASP.NET Core 8
│   ├── Controllers         # Endpoints REST (ej: ProductsController)
│   ├── Program.cs          # Configuración del pipeline
│   ├── appsettings.json              # Configuración común
│   ├── appsettings.Development.json  # Valores de desarrollo
│   └── appsettings.Production.json   # Valores de producción
│
└── ProyectoBase.Web        # Frontend en Angular 14
    ├── src/app
    │   ├── services        # ApiService para consumo de la API
    │   └── components      # ProductCrudComponent (CRUD completo)
    ├── angular.json
    └── package.json

```
## ⚙️ Configuración

### 1. Backend (API .NET 8)

```bash
cd ProyectoBase.Api
dotnet run

La API quedará disponible en:

Swagger: https://localhost:5001/swagger

Endpoints: https://localhost:5001/api/products

📌 CORS: se configura en appsettings.json (propiedad AllowedOrigins).

### 🌍 Variables de entorno y configuración

ASP.NET Core permite sobreescribir los archivos `appsettings*.json` mediante
variables de entorno con el prefijo `DOTNET_`. Esto es especialmente útil en
despliegues donde las credenciales no pueden vivir en el repositorio.

Los nombres de las variables se construyen reemplazando los dos puntos (`:`)
del `appsettings.json` por doble guion bajo (`__`). Ejemplos:

```bash
# Linux/macOS
export DOTNET_ConnectionStrings__DefaultConnection="Server=sql;Database=ProyectoBase;User Id=api;Password=${DB_PASSWORD};TrustServerCertificate=True;"
export DOTNET_Jwt__Issuer="https://api.midominio.com"
export DOTNET_Jwt__Audience="ProyectoBase.Web"
export DOTNET_Jwt__Key="${JWT_SECRET}"
export DOTNET_Redis__ConnectionString="redis:6379,abortConnect=false"
export DOTNET_Redis__InstanceName="ProyectoBase"

dotnet run --project ProyectoBase.Api

# Windows PowerShell
$env:DOTNET_ConnectionStrings__DefaultConnection = "Server=sql;Database=ProyectoBase;User Id=api;Password=$env:DB_PASSWORD;TrustServerCertificate=True;"
$env:DOTNET_Jwt__Issuer = "https://api.midominio.com"
$env:DOTNET_Jwt__Audience = "ProyectoBase.Web"
$env:DOTNET_Jwt__Key = $env:JWT_SECRET
$env:DOTNET_Redis__ConnectionString = "redis:6379,abortConnect=false"
$env:DOTNET_Redis__InstanceName = "ProyectoBase"

dotnet run --project ProyectoBase.Api
```

Si la aplicación se despliega en contenedores (Docker/Kubernetes) o servicios
gestionados (Azure App Service, AWS Elastic Beanstalk, etc.), basta con definir
estas mismas variables en el entorno de ejecución para que `Program.cs`
obtenga los valores en tiempo de arranque sin necesidad de modificarlos en el
código fuente.

### 📝 Logging con NLog

La API reemplaza el proveedor por defecto de ASP.NET Core y utiliza **NLog**
(`nlog.config` en `ProyectoBase.Api`) para centralizar los logs. Cada entorno
tiene un destino distinto:

- `Development`: salida estructurada en consola.
- `Staging`: consola + archivo.
- `Production`: solo archivo (`logs/<fecha>.log` por defecto).

Todos los mensajes pasan por un layout que anonimiza tokens, contraseñas y
valores sensibles detectados en mensajes, propiedades de log y cabeceras HTTP.

#### 🔧 Sobrescribir configuración de NLog por variables de entorno

Las variables declaradas en `nlog.config` permiten ajustar la configuración sin
editar archivos:

```bash
# Cambiar el nivel mínimo de log
export NLOG_MINLEVEL=Debug

# Redefinir el directorio de logs
export NLOG_LOG_DIRECTORY=/var/log/proyecto-base

dotnet run --project ProyectoBase.Api
```

Al iniciar la API, NLog leerá estas variables y adaptará los destinos de salida
con la configuración indicada.

#### ✅ Verificar logs de excepciones

El `ExceptionHandlingMiddleware` se mantiene al inicio del pipeline, por lo que
cualquier excepción no controlada termina en NLog con el formato anterior.

```bash
dotnet run --project ProyectoBase.Api

# En otra terminal generar un 404 para revisar el log estructurado
curl -k https://localhost:5001/api/products/99999
```

El middleware responde con un JSON estandarizado y el error queda registrado en
la consola o archivo según el entorno, sin exponer credenciales.

### 2. Frontend (Angular 14)
```bash

cd ProyectoBase.Web
npm install
ng serve -o
La aplicación se abrirá en http://localhost:4200
```

### 🗄️ Migraciones de Entity Framework Core

Para generar la migración inicial de la base de datos se debe ejecutar el siguiente comando desde la raíz del repositorio:

```bash
dotnet ef migrations add InitialCreate --project ProyectoBase.Infrastructure --startup-project ProyectoBase.Api --output-dir Persistence/Migrations
```

El comando utiliza el proyecto de infraestructura para almacenar las migraciones y el proyecto API como punto de entrada.

🧩 Funcionalidad de ejemplo
```bash
API (ProductsController)

GET /api/products → Lista todos los productos.

GET /api/products/{id} → Obtiene un producto por ID.

POST /api/products → Crea un producto.

PUT /api/products/{id} → Actualiza un producto existente.

DELETE /api/products/{id} → Elimina un producto.
```
🌐 Frontend
```bash
Listado de productos en tabla.

Formulario para crear y actualizar.

Acciones de editar y eliminar.

Conexión configurable a través de environment.ts.
```

🛠️ Requisitos
```
.NET SDK 8.0 → Descargar aquí https://dotnet.microsoft.com/download/dotnet/8.0

Node.js 16/18 → Descargar aquí https://nodejs.org/en/

Angular CLI 14 → instalar con: npm install -g @angular/cli@14
```

### 🧹 Estilo y análisis de código

- El archivo `.editorconfig` en la raíz define las reglas de formato (espacios en lugar de tabuladores, finales de línea `LF`, orden de `using`, etc.) y marca como error la falta de comentarios XML en el código productivo.
- Todos los proyectos .NET referencian `StyleCop.Analyzers` y habilitan `<EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>`, por lo que los avisos de estilo rompen la compilación si no se corrigen.
- Los proyectos de pruebas incluyen supresiones justificadas en `GlobalSuppressions.cs` para evitar la documentación XML obligatoria en pruebas unitarias o de integración, manteniendo el foco en la legibilidad de los escenarios.
- Para aplicar correcciones automáticas ejecuta:

```bash
dotnet format ProyectoBase.sln
```

Agrega la opción `--verify-no-changes` en CI para validar que el código enviado respeta el formato establecido.

### 🧪 Pruebas automatizadas

Ejecuta toda la suite (unitarias y de integración) desde la raíz del repositorio:

```bash
dotnet test ProyectoBase.sln
```

Este comando incluye:

- `ProyectoBase.Domain.Tests`: pruebas de dominio con **FluentAssertions** y mocks de repositorios utilizando **Moq**.
- `ProyectoBase.Api.IntegrationTests`: pruebas de integración que levantan la API mediante `WebApplicationFactory<Program>` y una base de datos SQLite en memoria.

📌 Próximos pasos
```
Integrar base de datos (EF Core o Dapper).
Autenticación con JWT.
CI/CD en GitHub Actions o Azure DevOps.
Pruebas unitarias y E2E.
```
