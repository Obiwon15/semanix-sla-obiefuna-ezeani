using Microsoft.AspNetCore.Mvc;
using MediatR;
using FormsService.Application.Commands;
using FormsService.Application.Queries;
using FormsService.Web.Models;
using System.Diagnostics.Metrics;

namespace FormsService.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FormsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FormsController> _logger;
    private readonly Counter<long> _formsPublishedCounter;

    public FormsController(IMediator mediator, ILogger<FormsController> logger, IMeterFactory meterFactory)
    {
        _mediator = mediator;
        _logger = logger;
        var meter = meterFactory.Create("FormsService");
        _formsPublishedCounter = meter.CreateCounter<long>("fe_forms_published_total", "Total number of forms published");
    }

    [HttpPost]
    public async Task<IActionResult> CreateForm([FromBody] CreateFormRequest request)
    {
        var tenantId = Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (string.IsNullOrEmpty(tenantId))
            return BadRequest(new ApiResponse { Success = false, Message = "X-Tenant-Id header is required" });

        var entityId = Request.Headers["X-Entity-Id"].FirstOrDefault();

        var command = new CreateFormCommand(
            tenantId,
            entityId,
            request.Name,
            request.Description,
            request.Sections);

        var result = await _mediator.Send(command);

        _logger.LogInformation("Form {FormId} created for tenant {TenantId}", result.Id, tenantId);

        return CreatedAtAction(nameof(GetForm), new { id = result.Id },
            new ApiResponse<object> { Success = true, Data = result });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetForm(Guid id)
    {
        var tenantId = Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (string.IsNullOrEmpty(tenantId))
            return BadRequest(new ApiResponse { Success = false, Message = "X-Tenant-Id header is required" });

        var query = new GetFormQuery(id, tenantId);
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound(new ApiResponse { Success = false, Message = "Form not found" });

        return Ok(new ApiResponse<object> { Success = true, Data = result });
    }

    [HttpGet]
    public async Task<IActionResult> GetForms([FromQuery] string? entityId = null)
    {
        var tenantId = Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (string.IsNullOrEmpty(tenantId))
            return BadRequest(new ApiResponse { Success = false, Message = "X-Tenant-Id header is required" });

        var query = new GetFormsQuery(tenantId, entityId);
        var result = await _mediator.Send(query);

        return Ok(new ApiResponse<object> { Success = true, Data = result });
    }

    [HttpPost("{id}/publish")]
    public async Task<IActionResult> PublishForm(Guid id)
    {
        var tenantId = Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (string.IsNullOrEmpty(tenantId))
            return BadRequest(new ApiResponse { Success = false, Message = "X-Tenant-Id header is required" });

        var command = new PublishFormCommand(id, tenantId);
        await _mediator.Send(command);

        _formsPublishedCounter.Add(1, new KeyValuePair<string, object?>("tenant_id", tenantId));
        _logger.LogInformation("Form {FormId} published for tenant {TenantId}", id, tenantId);

        return Ok(new ApiResponse { Success = true, Message = "Form published successfully" });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateForm(Guid id, [FromBody] UpdateFormRequest request)
    {
        var tenantId = Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (string.IsNullOrEmpty(tenantId))
            return BadRequest(new ApiResponse { Success = false, Message = "X-Tenant-Id header is required" });

        var command = new UpdateFormCommand(id, tenantId, request.Name, request.Description, request.Sections);
        var result = await _mediator.Send(command);

        _logger.LogInformation("Form {FormId} updated for tenant {TenantId}", id, tenantId);

        return Ok(new ApiResponse<object> { Success = true, Data = result });
    }

    [HttpPost("{id}/archive")]
    public async Task<IActionResult> ArchiveForm(Guid id)
    {
        var tenantId = Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (string.IsNullOrEmpty(tenantId))
            return BadRequest(new ApiResponse { Success = false, Message = "X-Tenant-Id header is required" });

        var command = new ArchiveFormCommand(id, tenantId);
        await _mediator.Send(command);

        _logger.LogInformation("Form {FormId} archived for tenant {TenantId}", id, tenantId);

        return Ok(new ApiResponse { Success = true, Message = "Form archived successfully" });
    }

    [HttpGet("/health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
}