using Microsoft.EntityFrameworkCore;
using RenderingService.Application.Services;
using RenderingService.Domain.Models;
using RenderingService.Infrastructure.Persistence;

namespace RenderingService.Infrastructure.Services;

public class RenderingServiceImpl : IRenderingService
{
    private readonly RenderingDbContext _context;

    public RenderingServiceImpl(RenderingDbContext context)
    {
        _context = context;
    }

    public async Task<List<RenderedForm>> GetFormsAsync(string tenantId, string? entityId = null)
    {
        var query = _context.RenderedForms.Where(f => f.TenantId == tenantId);

        if (!string.IsNullOrEmpty(entityId))
        {
            query = query.Where(f => f.EntityId == entityId);
        }

        return await query
            .OrderByDescending(f => f.UpdatedAt)
            .ToListAsync();
    }

    public async Task UpsertFormAsync(RenderedForm form)
    {
        var existing = await _context.RenderedForms
            .FirstOrDefaultAsync(f => f.Id == form.Id && f.TenantId == form.TenantId);

        if (existing == null)
        {
            await _context.RenderedForms.AddAsync(form);
        }
        else
        {
            existing.Name = form.Name;
            existing.Description = form.Description;
            existing.Version = form.Version;
            existing.FormDefinition = form.FormDefinition;
            existing.UpdatedAt = form.UpdatedAt;
        }

        await _context.SaveChangesAsync();
    }
}