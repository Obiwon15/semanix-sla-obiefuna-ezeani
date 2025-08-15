using FormsService.Application.Commands;
using FormsService.Application.DTOs;
using FormsService.Application.Validators;
using FormsService.Domain.Enums;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FormsService.Tests.Application;

public class ValidatorTests
{
    [Fact]
    public void CreateFormValidator_ShouldSucceed_WithValidCommand()
    {
        // Arrange
        var validator = new CreateFormValidator();
        var command = new CreateFormCommand(
            "tenant-123",
            "entity-456",
            "Valid Form",
            "Description",
            new List<SectionDto>
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
            });

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateFormValidator_ShouldFail_WithEmptyTenantId()
    {
        // Arrange
        var validator = new CreateFormValidator();
        var command = new CreateFormCommand(
            "",
            null,
            "Valid Form",
            "Description",
            new List<SectionDto>());

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TenantId");
    }

    [Fact]
    public void CreateFormValidator_ShouldFail_WithLongName()
    {
        // Arrange
        var validator = new CreateFormValidator();
        var command = new CreateFormCommand(
            "tenant-123",
            null,
            new string('A', 101), // 101 characters
            "Description",
            new List<SectionDto>
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
            });

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }
}

