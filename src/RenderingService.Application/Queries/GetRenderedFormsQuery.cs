using RenderingService.Application.Services;
using RenderingService.Domain.Models;

namespace RenderingService.Application.Queries;

public record GetRenderedFormsQuery(string TenantId, string? EntityId = null);

public class GetRenderedFormsHandler
{
    private readonly IRenderingService _renderingService;

    public GetRenderedFormsHandler(IRenderingService renderingService)
    {
        _renderingService = renderingService;
    }

    public async Task<List<RenderedForm>> HandleAsync(GetRenderedFormsQuery query)
    {
        return await _renderingService.GetFormsAsync(query.TenantId, query.EntityId);
    }
}