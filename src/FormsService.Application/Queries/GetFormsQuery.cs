using MediatR;
using FormsService.Application.DTOs;

namespace FormsService.Application.Queries;

public record GetFormsQuery(string TenantId, string? EntityId = null) : IRequest<List<FormDto>>;
