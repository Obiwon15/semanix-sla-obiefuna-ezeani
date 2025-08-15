using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RenderingService.Application.Queries;

namespace RenderingService.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RenderedFormsController : ControllerBase
{
    private readonly GetRenderedFormsHandler _handler;
    private readonly ILogger<RenderedFormsController> _logger;

    public RenderedFormsController(GetRenderedFormsHandler handler, ILogger<RenderedFormsController> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetRenderedForms([FromQuery] string tenant, [FromQuery] string? entity = null)
    {
        if (string.IsNullOrEmpty(tenant))
            return BadRequest(new { error = "tenant parameter is required" });

        var query = new GetRenderedFormsQuery(tenant, entity);
        var result = await _handler.HandleAsync(query);

        _logger.LogInformation("Retrieved {Count} rendered forms for tenant {TenantId}", result.Count, tenant);

        return Ok(new { success = true, data = result });
    }

    [HttpGet("/health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
}