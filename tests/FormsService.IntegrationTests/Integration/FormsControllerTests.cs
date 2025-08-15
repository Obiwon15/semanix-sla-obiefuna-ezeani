// tests/FormsService.Tests/Integration/FormsControllerTests.cs
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FormsService.Infrastructure.Persistence;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using FormsService.Web.Models;
using FormsService.Application.DTOs;
using FormsService.Domain.Enums;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using FieldType = FormsService.Domain.Enums.FieldType;

namespace FormsService.Tests.Integration;

public class FormsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public FormsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Replace the database with in-memory database
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<FormsDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<FormsDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateForm_ShouldReturnCreated_WithValidRequest()
    {
        // Arrange
        var request = new CreateFormRequest
        {
            Name = "Test Form",
            Description = "Test Description",
            Sections = new List<SectionDto>
            {
                new SectionDto
                {
                    Name = "Section 1",
                    Order = 1,
                    Fields = new List<FieldDefinitionDto>
                    {
                        new FieldDefinitionDto
                        {
                            FieldId = "field1",
                            Label = "Field 1",
                            Type = FieldType.Text,
                            Order = 1
                        }
                    }
                }
            }
        };

        _client.DefaultRequestHeaders.Add("X-Tenant-Id", "tenant-123");

        // Act
        var response = await _client.PostAsJsonAsync("/api/forms", request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);

        Assert.True(result.GetProperty("success").GetBoolean());
        Assert.True(result.GetProperty("data").GetProperty("id").TryGetGuid(out _));
    }

    [Fact]
    public async Task CreateForm_ShouldReturnBadRequest_WithoutTenantHeader()
    {
        // Arrange
        var request = new CreateFormRequest { Name = "Test Form", Sections = new List<SectionDto>() };

        // Act
        var response = await _client.PostAsJsonAsync("/api/forms", request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}
