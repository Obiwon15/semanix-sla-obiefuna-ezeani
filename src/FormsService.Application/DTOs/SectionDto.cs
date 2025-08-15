namespace FormsService.Application.DTOs;

public class SectionDto
{
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<FieldDefinitionDto> Fields { get; set; } = new();
}