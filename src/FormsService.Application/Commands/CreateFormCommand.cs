using MediatR;
using FormsService.Application.DTOs;

namespace FormsService.Application.Commands;

public record CreateFormCommand(
    string TenantId,
    string? EntityId,
    string Name,
    string? Description,
    List<SectionDto> Sections) : IRequest<FormDto>;