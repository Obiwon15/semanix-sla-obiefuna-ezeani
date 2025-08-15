using RenderingService.Domain.Models;

namespace RenderingService.Application.Services;

public interface IRenderingService
{
    Task<List<RenderedForm>> GetFormsAsync(string tenantId, string? entityId = null);
    Task UpsertFormAsync(RenderedForm form);
}