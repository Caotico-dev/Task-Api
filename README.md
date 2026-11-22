# Task API

API REST construida con ASP.NET Core 8 para gestionar usuarios, tareas, amistades y tareas asignadas a otros usuarios.

## Características

- Registro e inicio de sesión con ASP.NET Core Identity.
- Autenticación mediante JWT con expiración de 30 minutos.
- Cierre de sesión mediante lista negra en memoria.
- Gestión de tareas, prioridades, estados y asignaciones.
- Gestión de relaciones de amistad y estados de aprobación.
- Persistencia con Entity Framework Core y SQL Server.
- Registros de errores mediante Serilog y MongoDB.
- Documentación interactiva con Swagger en entorno de desarrollo.

## Requisitos

- .NET SDK 8.
- SQL Server.
- MongoDB, si se desea activar el registro de errores en MongoDB.
- Visual Studio o Visual Studio Code.

## Configuración

La aplicación usa `Mi_Task_Api/appsettings.json`.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=Mi_Tasks_Api;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "UNA_CLAVE_SECRETA_LARGA_Y_SEGURA",
    "Issuer": "http://localhost:5000",
    "Audience": "http://localhost:5000"
  }
}
```

Para producción se recomienda establecer el secreto JWT y la cadena de conexión mediante variables de entorno o un administrador de secretos, sin almacenarlos en el repositorio.

Ejemplo de variables de entorno en PowerShell:

```powershell
$env:ConnectionStrings__DefaultConnection = "Server=TU_SERVIDOR;Database=Mi_Tasks_Api;Trusted_Connection=True;TrustServerCertificate=True;"
$env:Jwt__Key = "UNA_CLAVE_SECRETA_LARGA_Y_SEGURA"
$env:Jwt__Issuer = "http://localhost:5055"
```

## Base de datos

El proyecto incluye migraciones de Entity Framework Core. Desde la carpeta raíz de la solución:

```powershell
dotnet tool restore
dotnet ef database update --project .\Mi_Task_Api\Mi_Task_Api.csproj
```

Si `dotnet-ef` no está instalado:

```powershell
dotnet tool install --global dotnet-ef
```

## Ejecución

Desde la raíz del repositorio:

```powershell
dotnet restore .\Mi_Task_Api.sln
dotnet run --project .\Mi_Task_Api\Mi_Task_Api.csproj --launch-profile https
```

URLs configuradas:

- HTTP: `http://localhost:5055`
- HTTPS: `https://localhost:7147`
- Swagger: `https://localhost:7147/swagger`

Swagger solo está habilitado cuando `ASPNETCORE_ENVIRONMENT=Development`.

## Uso básico

### 1. Registrar un usuario

```http
POST /registar
Content-Type: application/json

{
  "name": "usuario",
  "email": "usuario@example.com",
  "password": "ClaveSegura123"
}
```

### 2. Iniciar sesión

```http
POST /sign
Content-Type: application/json

{
  "email": "usuario@example.com",
  "password": "ClaveSegura123"
}
```

La respuesta contiene el cuaderno del usuario, sus tareas, amistades y asignaciones, además del campo `token`.

### 3. Consumir endpoints protegidos

```http
Authorization: Bearer <token>
```

### 4. Cerrar sesión

```http
POST /signout
Authorization: Bearer <token>
```

El token enviado queda registrado en la lista negra mientras la aplicación permanece activa.

## Estados admitidos

Estados de una relación o asignación:

- `Pending`
- `Accepted`
- `Rejected`
- `Block`

Estados de una tarea:

- `Pending`
- `InProgress`
- `Completed`

Prioridades de una tarea:

- `High`
- `Medium`
- `Low`

Los valores desconocidos se normalizan automáticamente: prioridad y estado de tarea a `Low`/`Pending`, y estado de relación o asignación a `Pending`.

## Arquitectura

```text
Mi_Task_Api/
├── Authentication/       JWT, lista negra y middleware
├── Controllers/          Endpoints HTTP
├── Managers/             Reglas de negocio y acceso a datos
├── Migrations/           Migraciones de Entity Framework
├── Model/                Entidades y DbContext
├── ModelDto/             Contratos de entrada y salida
├── Properties/           Configuración de ejecución
└── Program.cs            Registro de servicios y pipeline
```

Consulte [la documentación completa de la API](docs/API.md) para conocer todos los endpoints y contratos.

## Compilación

```powershell
dotnet build .\Mi_Task_Api.sln
```

## Pruebas

El repositorio no contiene actualmente un proyecto de pruebas. Los endpoints pueden validarse desde Swagger o con el cliente HTTP incluido en el proyecto.
