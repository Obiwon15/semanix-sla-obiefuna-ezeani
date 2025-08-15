using FormsService.Application.DTOs;

namespace FormsService.Application.Services;

public interface IFormQueryService
{
    Task<FormDto?> GetFormAsync(Guid formId, string tenantId, CancellationToken cancellationToken = default);
    Task<List<FormDto>> GetFormsAsync(string tenantId, string? entityId = null, CancellationToken cancellationToken = default);
}