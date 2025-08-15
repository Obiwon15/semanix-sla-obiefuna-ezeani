using MediatR;
using FormsService.Application.DTOs;

namespace FormsService.Application.Commands;

public record UpdateFormCommand(
    Guid FormId,
    string TenantId,
    string Name,
    string? Description,
    List<SectionDto> Sections) : IRequest<FormDto>;