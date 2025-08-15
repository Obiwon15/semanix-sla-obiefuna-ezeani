using FormsService.Domain.Events;
using FormsService.Domain.Exceptions;
using FormsService.Domain.ValueObjects;
using FormsService.Domain.Enums;
using static System.Collections.Specialized.BitVector32;
using Section = FormsService.Domain.ValueObjects.Section;

namespace FormsService.Domain.Aggregates;

public class Form : AggregateRoot
{
    public FormId Id { get; private set; }
    public TenantId TenantId { get; private set; }
    public EntityId? EntityId { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public FormStatus Status { get; private set; }
    public int Version { get; private set; }
    public List<ValueObjects.Section> Sections { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // EF Core constructor
    private Form() { }

    public Form(FormId id, TenantId tenantId, EntityId? entityId, string name, string? description, List<Section> sections)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        EntityId = entityId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        Status = FormStatus.Draft;
        Version = 1;
        Sections = sections ?? throw new ArgumentNullException(nameof(sections));
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        ValidateForm();
    }

    public void Publish()
    {
        if (Status != FormStatus.Draft)
        {
            throw new InvalidTransitionException($"Cannot publish form in {Status} status. Only Draft forms can be published.");
        }

        Status = FormStatus.Published;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new FormPublishedEvent(Id.Value, TenantId.Value, EntityId?.Value, Version));
    }

    public void UpdateAndPublish(string name, string? description, List<Section> sections)
    {
        if (Status != FormStatus.Published)
        {
            throw new InvalidTransitionException($"Cannot update form in {Status} status. Only Published forms can be updated.");
        }

        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        Sections = sections ?? throw new ArgumentNullException(nameof(sections));
        Version++;
        UpdatedAt = DateTime.UtcNow;

        ValidateForm();
        AddDomainEvent(new FormUpdatedEvent(Id.Value, TenantId.Value, EntityId?.Value, Version));
    }

    public void Archive()
    {
        if (Status == FormStatus.Archived)
        {
            throw new InvalidTransitionException("Form is already archived.");
        }

        if (Status != FormStatus.Draft && Status != FormStatus.Published)
        {
            throw new InvalidTransitionException($"Cannot archive form in {Status} status.");
        }

        Status = FormStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }

    private void ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new DomainException("Form name cannot be empty.");

        if (Name.Length > 100)
            throw new DomainException("Form name cannot exceed 100 characters.");

        if (Sections == null || !Sections.Any())
            throw new DomainException("Form must have at least one section.");

        ValidateSections();
    }

    private void ValidateSections()
    {
        foreach (var section in Sections)
        {
            if (section.Fields == null || !section.Fields.Any())
                throw new DomainException($"Section '{section.Name}' must have at least one field.");

            var orderValues = section.Fields.Select(f => f.Order).ToList();
            if (orderValues.Count != orderValues.Distinct().Count())
                throw new DomainException($"Section '{section.Name}' has duplicate field orders.");

            foreach (var field in section.Fields)
            {
                if (string.IsNullOrWhiteSpace(field.FieldId))
                    throw new DomainException($"Field in section '{section.Name}' must have a FieldId.");

                if (string.IsNullOrWhiteSpace(field.Label))
                    throw new DomainException($"Field '{field.FieldId}' must have a Label.");
            }
        }
    }
}
