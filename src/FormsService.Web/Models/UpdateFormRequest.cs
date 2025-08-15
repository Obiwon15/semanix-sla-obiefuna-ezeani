using FormsService.Application.DTOs;

namespace FormsService.Web.Models;

public class UpdateFormRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<SectionDto> Sections { get; set; } = new();
}