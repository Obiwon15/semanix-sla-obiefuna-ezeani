using FormsService.Domain.Aggregates;
using FormsService.Domain.ValueObjects;

namespace FormsService.Domain.Repositories;

public interface IFormRepository
{
    Task<Form?> GetByIdAsync(FormId id, TenantId tenantId, CancellationToken cancellationToken = default);
    Task<Form> AddAsync(Form form, CancellationToken cancellationToken = default);
    Task UpdateAsync(Form form, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(FormId id, TenantId tenantId, CancellationToken cancellationToken = default);
}