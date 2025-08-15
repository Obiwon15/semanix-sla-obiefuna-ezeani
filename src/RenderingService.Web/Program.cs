using RabbitMQ.Client;
using RenderingService.Application.Handlers;
using RenderingService.Application.Queries;
using RenderingService.Application.Services;
using Microsoft.EntityFrameworkCore;
using RenderingService.Infrastructure.Messaging;
using RenderingService.Infrastructure.Persistence;
using RenderingService.Infrastructure.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<RenderingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<IRenderingService, RenderingServiceImpl>();
builder.Services.AddScoped<FormPublishedHandler>();
builder.Services.AddScoped<FormUpdatedHandler>();
builder.Services.AddScoped<GetRenderedFormsHandler>();
// RabbitMQ
builder.Services.AddSingleton<IConnection>(provider =>
{
    var factory = new ConnectionFactory
    {
        Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ")!)
    };
    return factory.CreateConnection();
});

builder.Services.AddHostedService<RabbitMqConsumer>();

// Health checks
builder.Services.AddHealthChecks();
    builder.Services.AddDbContext<RenderingDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();
app.MapHealthChecks("/health");

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<RenderingDbContext>();
    context.Database.EnsureCreated();
}

Log.Information("Rendering Service starting up");

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

