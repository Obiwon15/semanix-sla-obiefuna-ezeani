using FormsService.Application.Commands;
using FormsService.Application.DTOs;
using FormsService.Application.Handlers;
using FormsService.Application.Services;
using FormsService.Application.Validators;
using FormsService.Domain.Aggregates;
using FormsService.Domain.Enums;
using FormsService.Domain.Events;
using FormsService.Domain.Repositories;
using FormsService.Domain.ValueObjects;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FormsService.Tests.Application;

public class CreateFormHandlerTests
{
    private readonly Mock<IFormRepository> _mockRepository;
    private readonly Mock<IEventPublisher> _mockEventPublisher;
    private readonly CreateFormHandler _handler;

    public CreateFormHandlerTests()
    {
        _mockRepository = new Mock<IFormRepository>();
        _mockEventPublisher = new Mock<IEventPublisher>();
        _handler = new CreateFormHandler(_mockRepository.Object, _mockEventPublisher.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateForm_WithValidCommand()
    {
        // Arrange
        var command = new CreateFormCommand(
            "tenant-123",
            "entity-456",
            "Test Form",
            "Test Description",
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

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Form>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Form f, CancellationToken ct) => f);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Form", result.Name);
        Assert.Equal("Test Description", result.Description);
        Assert.Equal(FormStatus.Draft, result.Status);
        Assert.Equal(1, result.Version);

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Form>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockEventPublisher.Verify(e => e.PublishDomainEventsAsync(It.IsAny<IEnumerable<IDomainEvent>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}

