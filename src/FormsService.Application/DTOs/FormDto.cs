using FormsService.Domain.Enums;

namespace FormsService.Application.DTOs;

public class FormDto
{
    public Guid Id { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public FormStatus Status { get; set; }
    public int Version { get; set; }
    public List<SectionDto> Sections { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}