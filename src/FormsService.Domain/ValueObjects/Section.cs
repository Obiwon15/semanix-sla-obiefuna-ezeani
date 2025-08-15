using System.Reflection.Metadata;

namespace FormsService.Domain.ValueObjects;

public class Section
{
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<FieldDefinition> Fields { get; set; } = new();
}