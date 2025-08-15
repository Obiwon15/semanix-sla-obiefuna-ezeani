namespace RenderingService.Domain.Models;
public class RenderedForm
{
    public Guid Id { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Version { get; set; }
    public string FormDefinition { get; set; } = string.Empty; // JSON
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}