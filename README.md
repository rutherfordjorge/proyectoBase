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
│   └── appsettings.json    # Configuración (AllowedOrigins para CORS)
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

```
### 2. Frontend (Angular 14)
```bash

cd ProyectoBase.Web
npm install
ng serve -o
La aplicación se abrirá en http://localhost:4200
```

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
📌 Próximos pasos
```
Integrar base de datos (EF Core o Dapper).
Autenticación con JWT.
CI/CD en GitHub Actions o Azure DevOps.
Pruebas unitarias y E2E.
```
