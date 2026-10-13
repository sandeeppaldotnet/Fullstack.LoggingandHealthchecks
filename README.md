# Fullstack.LoggingandHealthchecks

A small .NET 10 project providing health checks and logging integration patterns for ASP.NET Core applications.

Table of Contents
- Overview
- Features
- Prerequisites
- Getting started
  - Clone
  - Build
  - Run
- Health checks
  - Registering health checks
  - Example: CatalogDependencyHealthCheck
- Logging
- Configuration
- Development & testing
- Contributing
- License

Overview
--------
This repository demonstrates standard patterns for adding health checks and structured logging to an ASP.NET Core application targeting .NET 10. It includes an example health check (CatalogDependencyHealthCheck) that can be registered with the ASP.NET Core health-check system.

Features
--------
- .NET 10 project structure
- Health check example(s)
- Guidance for registering and exposing health endpoints
- Logging configuration examples

Prerequisites
-------------
- .NET SDK 10.x (install from https://dotnet.microsoft.com)
- Visual Studio 2026 (optional) or any editor that supports .NET 10

Getting started
---------------
Clone the repository:

```powershell
git clone <repo-url> Fullstack.LoggingandHealthchecks
cd "Fullstack.LoggingandHealthchecks"
```

Restore, build and run using the .NET CLI:

```powershell
dotnet restore
dotnet build --configuration Release
dotnet run --project src/YourWebProjectFolder --configuration Release
```

Replace src/YourWebProjectFolder with the path to the ASP.NET Core project that hosts the health checks (if the repository contains multiple projects).

Running in Visual Studio
------------------------
Open the solution (.sln) in Visual Studio 2026 and start the project that contains the ASP.NET Core host (F5 or Ctrl+F5).

Health checks
-------------
This project includes an example health check (CatalogDependencyHealthCheck). To expose health checks in your ASP.NET Core application, register health checks in Program.cs or Startup.cs and map an endpoint.

Example (minimal Program.cs for .NET 10):

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register health checks
builder.Services.AddHealthChecks()
	.AddCheck<CatalogDependencyHealthCheck>("catalog_dependency");

var app = builder.Build();

// Map health checks endpoint
app.MapHealthChecks("/health");

app.Run();
```

If CatalogDependencyHealthCheck depends on configuration or HTTP clients, register them before adding the health check. Example if it uses an HttpClient:

```csharp
builder.Services.AddHttpClient("catalogClient", client =>
{
	client.BaseAddress = new Uri(builder.Configuration["Catalog:BaseUrl"] ?? "https://example.com/");
});n
builder.Services.AddHealthChecks().AddCheck<CatalogDependencyHealthCheck>("catalog_dependency");
```

Health check implementation notes
- Health check classes should implement Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck and return HealthCheckResult. Keep checks fast and deterministic.
- Use appropriate failure statuses and include useful descriptions for diagnostics.

Logging
-------
This repository does not mandate a specific logging provider. .NET's built-in ILogger<T> works well. For structured logging and sinks consider Serilog:

Example Serilog bootstrap (Program.cs):

```csharp
Log.Logger = new LoggerConfiguration()
	.Enrich.FromLogContext()
	.WriteTo.Console()
	.CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, services, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));
```

Configuration
-------------
Use appsettings.json and environment variables as usual. Example configuration keys used by health checks might include:

```json
{
  "Catalog": {
	"BaseUrl": "https://catalog.example.local/"
  }
}
```

Development & testing
---------------------
- Add unit tests for health check logic: verify healthy/unhealthy states without calling live dependencies by mocking HTTP clients or using TestServer where appropriate.
- Run tests with:

```powershell
dotnet test
```

Contributing
------------
- Open issues for bugs or feature requests
- Fork the repo, create a feature branch, and submit pull requests

License
-------
Specify the project license here (e.g., MIT) or add a LICENSE file to the repository.

Contact
-------
For questions about this repository, open an issue or contact the maintainers listed in repository metadata.

---

Notes
-----
- Adjust paths and project names to match this repository layout. The health-check example name CatalogDependencyHealthCheck is used here because a file with that name exists in the workspace.
- Keep health check execution inexpensive and use caching or rate limiting if the check requires external calls.
