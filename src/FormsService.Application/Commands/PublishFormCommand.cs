using MediatR;

namespace FormsService.Application.Commands;

public record PublishFormCommand(Guid FormId, string TenantId) : IRequest;