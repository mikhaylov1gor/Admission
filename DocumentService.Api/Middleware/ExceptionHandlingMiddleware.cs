using System.Net;
using Contract.Application.Exceptions;
namespace DocumentService.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await Handle(context, ex);
        }
    }

    private async Task Handle(HttpContext context, Exception ex)
    {
        HttpStatusCode status;
        Object response;

        switch (ex)
        {
            case ValidationException validationException:
                status = HttpStatusCode.BadRequest;
                response = new { error = validationException.Message };
                _logger.LogWarning(ex, "Validation error");
                break;
            case Contract.Application.Exceptions.UnauthorizedAccessException unauthorizedAccessException:
                status = HttpStatusCode.Unauthorized;
                response = new { error = unauthorizedAccessException.Message };
                _logger.LogWarning(ex, "Unauthorized");
                break;
            case ForbiddenAccessException forbiddenAccessException:
                status = HttpStatusCode.Forbidden;
                response = new { error = forbiddenAccessException.Message };
                _logger.LogWarning(ex, "Forbidden");
                break;
            case NotFoundException notFoundException:
                status = HttpStatusCode.NotFound;
                response = new { error = notFoundException.Message };
                _logger.LogWarning(ex, "Not found");
                break;
            
            default:
                status = HttpStatusCode.InternalServerError;
                response = new { error = "Internal server error" };
                _logger.LogError(ex, "Internal server error");
                break;
        }
        
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;
        await context.Response.WriteAsJsonAsync(response);
    }
}