using FormsService.Domain.Enums;
using Microsoft.VisualBasic.FileIO;

namespace FormsService.Domain.ValueObjects;

public class FieldDefinition
{
    public string FieldId { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public Enums.FieldType Type { get; set; }
    public int Order { get; set; }
    public Dictionary<string, object> ValidationRules { get; set; } = new();
}
