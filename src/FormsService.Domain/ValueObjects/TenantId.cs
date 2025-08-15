namespace FormsService.Domain.ValueObjects;

public record TenantId(string Value)
{
    public static implicit operator string(TenantId tenantId) => tenantId.Value;
    public override string ToString() => Value;
}