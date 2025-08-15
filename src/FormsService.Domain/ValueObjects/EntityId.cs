namespace FormsService.Domain.ValueObjects;

public record EntityId(string Value)
{
    public static implicit operator string(EntityId entityId) => entityId.Value;
    public override string ToString() => Value;
}