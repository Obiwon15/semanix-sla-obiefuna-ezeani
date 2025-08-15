using FormsService.Domain.Enums;

namespace FormsService.Application.DTOs;

public class FieldDefinitionDto
{
    public string FieldId { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public FieldType Type { get; set; }
    public int Order { get; set; }
    public Dictionary<string, object> ValidationRules { get; set; } = new();
}