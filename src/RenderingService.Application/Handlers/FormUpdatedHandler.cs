using RenderingService.Application.Services;
using RenderingService.Domain.Models;
using Microsoft.Extensions.Logging;

namespace RenderingService.Application.Handlers;

public class FormUpdatedHandler
{
    private readonly IRenderingService _renderingService;
    private readonly ILogger<FormUpdatedHandler> _logger;

    public FormUpdatedHandler(IRenderingService renderingService, ILogger<FormUpdatedHandler> logger)
    {
        _renderingService = renderingService;
        _logger = logger;
    }

    public async Task HandleAsync(FormUpdatedEvent eventData)
    {
        try
        {
            _logger.LogInformation("Processing FormUpdated event for form {FormId}, version {Version}",
                eventData.FormId, eventData.Version);

            var renderedForm = new RenderedForm
            {
                Id = eventData.FormId,
                TenantId = eventData.TenantId,
                EntityId = eventData.EntityId,
                Name = $"Form {eventData.FormId} v{eventData.Version}",
                Version = eventData.Version,
                FormDefinition = "{}",
                UpdatedAt = DateTime.UtcNow
            };

            await _renderingService.UpsertFormAsync(renderedForm);

            _logger.LogInformation("Successfully processed FormUpdated event for form {FormId}", eventData.FormId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process FormUpdated event for form {FormId}", eventData.FormId);
            throw;
        }
    }
}

public record FormUpdatedEvent(Guid FormId, string TenantId, string? EntityId, int Version);
