using FormsService.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace FormsService.Web.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            DomainException domainEx => new
            {
                error = domainEx.Message,
                statusCode = HttpStatusCode.BadRequest
            },
            //InvalidTransitionException transitionEx => new
            //{
            //    error = transitionEx.Message,
            //    statusCode = HttpStatusCode.BadRequest
            //},
            _ => new
            {
                error = "An error occurred while processing your request",
                statusCode = HttpStatusCode.InternalServerError
            }
        };

        context.Response.StatusCode = (int)response.statusCode;

        var jsonResponse = JsonSerializer.Serialize(new
        {
            success = false,
            message = response.error
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}