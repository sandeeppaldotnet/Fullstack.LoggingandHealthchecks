using Fullstack.LoggingandHealthchecks.Health;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks()
    .AddCheck<CatalogDependencyHealthCheck>(
        "catalog-dependency",
        tags: new[] { "ready" });



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler("/api/error");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
