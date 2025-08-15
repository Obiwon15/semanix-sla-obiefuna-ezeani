// tests/RenderingService.Tests/Integration/RenderingIntegrationTests.cs
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RenderingService.Infrastructure.Persistence;
using RenderingService.Domain.Models;
using System.Text.Json;
using Xunit;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace RenderingService.Tests.Integration;

public class RenderingIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public RenderingIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<RenderingDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<RenderingDbContext>(options =>
                    options.UseInMemoryDatabase("RenderingTestDb"));
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetRenderedForms_ShouldReturnForms_ForValidTenant()
    {
        // Arrange
        await SeedTestData();

        // Act
        var response = await _client.GetAsync("/api/rendered-forms?tenant=tenant-123");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);

        Assert.True(result.GetProperty("success").GetBoolean());
        Assert.True(result.GetProperty("data").GetArrayLength() > 0);
    }

    [Fact]
    public async Task GetRenderedForms_ShouldReturnBadRequest_WithoutTenantParameter()
    {
        // Act
        var response = await _client.GetAsync("/api/rendered-forms");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task SeedTestData()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RenderingDbContext>();

        context.RenderedForms.Add(new RenderedForm
        {
            Id = Guid.NewGuid(),
            TenantId = "tenant-123",
            EntityId = "entity-456",
            Name = "Test Form",
            Description = "Test Description",
            Version = 1,
            FormDefinition = "{}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await context.SaveChangesAsync();
    }
}
