namespace FormsService.Domain.ValueObjects;

public record FormId(Guid Value)
{
    public static FormId New() => new(Guid.NewGuid());
    public static implicit operator Guid(FormId formId) => formId.Value;
    public override string ToString() => Value.ToString();
}