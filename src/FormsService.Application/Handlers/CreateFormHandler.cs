using MediatR;
using FormsService.Application.Commands;
using FormsService.Application.DTOs;
using FormsService.Application.Services;
using FormsService.Domain.Aggregates;
using FormsService.Domain.Repositories;
using FormsService.Domain.ValueObjects;

namespace FormsService.Application.Handlers;

public class CreateFormHandler : IRequestHandler<CreateFormCommand, FormDto>
{
    private readonly IFormRepository _formRepository;
    private readonly IEventPublisher _eventPublisher;

    public CreateFormHandler(IFormRepository formRepository, IEventPublisher eventPublisher)
    {
        _formRepository = formRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<FormDto> Handle(CreateFormCommand request, CancellationToken cancellationToken)
    {
        var formId = FormId.New();
        var tenantId = new TenantId(request.TenantId);
        var entityId = request.EntityId != null ? new EntityId(request.EntityId) : null;

        var sections = request.Sections.Select(s => new Section
        {
            Name = s.Name,
            Order = s.Order,
            Fields = s.Fields.Select(f => new FieldDefinition
            {
                FieldId = f.FieldId,
                Label = f.Label,
                Type = f.Type,
                Order = f.Order,
                ValidationRules = f.ValidationRules
            }).ToList()
        }).ToList();

        var form = new Form(formId, tenantId, entityId, request.Name, request.Description, sections);

        await _formRepository.AddAsync(form, cancellationToken);
        await _eventPublisher.PublishDomainEventsAsync(form.DomainEvents, cancellationToken);

        return MapToDto(form);
    }

    private static FormDto MapToDto(Form form) => new()
    {
        Id = form.Id.Value,
        TenantId = form.TenantId.Value,
        EntityId = form.EntityId?.Value,
        Name = form.Name,
        Description = form.Description,
        Status = form.Status,
        Version = form.Version,
        CreatedAt = form.CreatedAt,
        UpdatedAt = form.UpdatedAt,
        Sections = form.Sections.Select(s => new SectionDto
        {
            Name = s.Name,
            Order = s.Order,
            Fields = s.Fields.Select(f => new FieldDefinitionDto
            {
                FieldId = f.FieldId,
                Label = f.Label,
                Type = f.Type,
                Order = f.Order,
                ValidationRules = f.ValidationRules
            }).ToList()
        }).ToList()
    };
}