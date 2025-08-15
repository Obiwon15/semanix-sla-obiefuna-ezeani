using MediatR;
using FormsService.Application.DTOs;

namespace FormsService.Application.Queries;

public record GetFormQuery(Guid FormId, string TenantId) : IRequest<FormDto?>;