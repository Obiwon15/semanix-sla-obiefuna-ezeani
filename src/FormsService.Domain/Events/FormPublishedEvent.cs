namespace FormsService.Domain.Events;

public record FormPublishedEvent(
    Guid FormId,
    string TenantId,
    string? EntityId,
    int Version) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
