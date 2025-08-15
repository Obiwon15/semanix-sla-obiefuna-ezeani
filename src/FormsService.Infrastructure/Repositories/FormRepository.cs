using Microsoft.EntityFrameworkCore;
using FormsService.Domain.Aggregates;
using FormsService.Domain.Repositories;
using FormsService.Domain.ValueObjects;
using FormsService.Infrastructure.Persistence;

namespace FormsService.Infrastructure.Repositories;

public class FormRepository : IFormRepository
{
    private readonly FormsDbContext _context;

    public FormRepository(FormsDbContext context)
    {
        _context = context;
    }

    public async Task<Form?> GetByIdAsync(FormId id, TenantId tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Forms
            .FirstOrDefaultAsync(f => f.Id == id && f.TenantId == tenantId, cancellationToken);
    }

    public async Task<Form> AddAsync(Form form, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Forms.AddAsync(form, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entry.Entity;
    }

    public async Task UpdateAsync(Form form, CancellationToken cancellationToken = default)
    {
        _context.Forms.Update(form);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(FormId id, TenantId tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Forms
            .AnyAsync(f => f.Id == id && f.TenantId == tenantId, cancellationToken);
    }
}