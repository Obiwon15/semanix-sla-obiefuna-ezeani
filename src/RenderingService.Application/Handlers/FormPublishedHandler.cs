using RenderingService.Application.Services;
using RenderingService.Domain.Models;
using Microsoft.Extensions.Logging;
using Serilog;

namespace RenderingService.Application.Handlers;

public class FormPublishedHandler
{
    private readonly IRenderingService _renderingService;
    private readonly ILogger<FormPublishedHandler> _logger;

    public FormPublishedHandler(IRenderingService renderingService, ILogger<FormPublishedHandler> logger)
    {
        _renderingService = renderingService;
        _logger = logger;
    }

    public async Task HandleAsync(FormPublishedEvent eventData)
    {
        try
        {
            _logger.LogInformation("Processing FormPublished event for form {FormId}, version {Version}",
                eventData.FormId, eventData.Version);

            // In a real implementation, you'd fetch the full form data from the Forms Service
            // For this demo, we'll create a simplified representation
            var renderedForm = new RenderedForm
            {
                Id = eventData.FormId,
                TenantId = eventData.TenantId,
                EntityId = eventData.EntityId,
                Name = $"Form {eventData.FormId}",
                Version = eventData.Version,
                FormDefinition = "{}", // Would contain the actual form structure
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _renderingService.UpsertFormAsync(renderedForm);

            _logger.LogInformation("Successfully processed FormPublished event for form {FormId}", eventData.FormId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process FormPublished event for form {FormId}", eventData.FormId);
            throw;
        }
    }
}

public record FormPublishedEvent(Guid FormId, string TenantId, string? EntityId, int Version);
