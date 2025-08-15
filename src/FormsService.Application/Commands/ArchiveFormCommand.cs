using MediatR;

namespace FormsService.Application.Commands;

public record ArchiveFormCommand(Guid FormId, string TenantId) : IRequest;