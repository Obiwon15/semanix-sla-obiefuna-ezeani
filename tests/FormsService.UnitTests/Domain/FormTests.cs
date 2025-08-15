using FormsService.Application.Commands;
using FormsService.Application.DTOs;
using FormsService.Application.Handlers;
using FormsService.Application.Services;
using FormsService.Application.Validators;
using FormsService.Domain.Aggregates;
using FormsService.Domain.Enums;
using FormsService.Domain.Exceptions;
using FormsService.Domain.Repositories;
using FormsService.Domain.ValueObjects;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FormsService.Tests.Domain;

public class FormTests
{
    [Fact]
    public void Form_Creation_ShouldSucceed_WithValidData()
    {
        // Arrange
        var formId = FormId.New();
        var tenantId = new TenantId("tenant-123");
        var entityId = new EntityId("entity-456");
        var sections = CreateValidSections();

        // Act
        var form = new Form(formId, tenantId, entityId, "Test Form", "Description", sections);

        // Assert
        Assert.Equal(formId, form.Id);
        Assert.Equal(tenantId, form.TenantId);
        Assert.Equal(entityId, form.EntityId);
        Assert.Equal("Test Form", form.Name);
        Assert.Equal("Description", form.Description);
        Assert.Equal(FormStatus.Draft, form.Status);
        Assert.Equal(1, form.Version);
        Assert.Equal(sections.Count, form.Sections.Count);
    }

    [Fact]
    public void Form_Publish_ShouldSucceed_FromDraftStatus()
    {
        // Arrange
        var form = CreateValidForm();

        // Act
        form.Publish();

        // Assert
        Assert.Equal(FormStatus.Published, form.Status);
        Assert.Single(form.DomainEvents);
        Assert.IsType<FormsService.Domain.Events.FormPublishedEvent>(form.DomainEvents.First());
    }

    [Fact]
    public void Form_Publish_ShouldFail_FromPublishedStatus()
    {
        // Arrange
        var form = CreateValidForm();
        form.Publish();
        form.ClearDomainEvents();

        // Act & Assert
        var exception = Assert.Throws<InvalidTransitionException>(() => form.Publish());
        Assert.Contains("Cannot publish form in Published status", exception.Message);
    }

    [Fact]
    public void Form_UpdateAndPublish_ShouldSucceed_FromPublishedStatus()
    {
        // Arrange
        var form = CreateValidForm();
        form.Publish();
        form.ClearDomainEvents();
        var newSections = CreateValidSections();

        // Act
        form.UpdateAndPublish("Updated Form", "Updated Description", newSections);

        // Assert
        Assert.Equal("Updated Form", form.Name);
        Assert.Equal("Updated Description", form.Description);
        Assert.Equal(2, form.Version);
        Assert.Single(form.DomainEvents);
        Assert.IsType<FormsService.Domain.Events.FormUpdatedEvent>(form.DomainEvents.First());
    }

    [Fact]
    public void Form_Archive_ShouldSucceed_FromDraftStatus()
    {
        // Arrange
        var form = CreateValidForm();

        // Act
        form.Archive();

        // Assert
        Assert.Equal(FormStatus.Archived, form.Status);
    }

    [Fact]
    public void Form_Archive_ShouldSucceed_FromPublishedStatus()
    {
        // Arrange
        var form = CreateValidForm();
        form.Publish();

        // Act
        form.Archive();

        // Assert
        Assert.Equal(FormStatus.Archived, form.Status);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Form_Creation_ShouldFail_WithInvalidName(string invalidName)
    {
        // Arrange
        var formId = FormId.New();
        var tenantId = new TenantId("tenant-123");
        var sections = CreateValidSections();

        // Act & Assert
        Assert.Throws<DomainException>(() =>
            new Form(formId, tenantId, null, invalidName, null, sections));
    }

    [Fact]
    public void Form_Creation_ShouldFail_WithDuplicateFieldOrders()
    {
        // Arrange
        var formId = FormId.New();
        var tenantId = new TenantId("tenant-123");
        var sections = new List<Section>
        {
            new Section
            {
                Name = "Section 1",
                Order = 1,
                Fields = new List<FieldDefinition>
                {
                    new FieldDefinition { FieldId = "field1", Label = "Field 1", Type = FieldType.Text, Order = 1 },
                    new FieldDefinition { FieldId = "field2", Label = "Field 2", Type = FieldType.Text, Order = 1 } // Duplicate order
                }
            }
        };

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            new Form(formId, tenantId, null, "Test Form", null, sections));
        Assert.Contains("duplicate field orders", exception.Message);
    }

    private static Form CreateValidForm()
    {
        return new Form(
            FormId.New(),
            new TenantId("tenant-123"),
            new EntityId("entity-456"),
            "Test Form",
            "Description",
            CreateValidSections());
    }

    private static List<Section> CreateValidSections()
    {
        return new List<Section>
        {
            new Section
            {
                Name = "Personal Info",
                Order = 1,
                Fields = new List<FieldDefinition>
                {
                    new FieldDefinition
                    {
                        FieldId = "full-name",
                        Label = "Full Name",
                        Type = FieldType.Text,
                        Order = 1,
                        ValidationRules = new Dictionary<string, object> { { "required", true } }
                    }
                }
            }
        };
    }
}
