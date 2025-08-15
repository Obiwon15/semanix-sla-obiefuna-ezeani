namespace FormsService.Web.Middleware;

public class TenantValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantValidationMiddleware> _logger;

    public TenantValidationMiddleware(RequestDelegate next, ILogger<TenantValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip validation for health checks and swagger
        if (context.Request.Path.StartsWithSegments("/health") ||
            context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }

        // Only validate API endpoints
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            var tenantId = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
            if (string.IsNullOrEmpty(tenantId))
            {
                _logger.LogWarning("Request to {Path} missing required X-Tenant-Id header", context.Request.Path);
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("X-Tenant-Id header is required");
                return;
            }

            _logger.LogDebug("Request validated for tenant {TenantId}", tenantId);
        }

        await _next(context);
    }
}