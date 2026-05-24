using System.Net;
using System.Text.Json;

namespace TankiForum.Middleware;

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
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Forbidden: {Method} {Path}", context.Request.Method, context.Request.Path);
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            await WriteErrorAsync(context, "Forbidden", ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Conflict: {Method} {Path}", context.Request.Method, context.Request.Path);
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            await WriteErrorAsync(context, "Conflict", ex.Message);
        }
        catch (IOException ex) when (ex.GetType().Name == "BadHttpRequestException")
        {
            _logger.LogWarning(ex, "Bad request: {Method} {Path}", context.Request.Method, context.Request.Path);
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await WriteErrorAsync(context, "Bad Request", "Request content could not be read. Check Content-Length and body format.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Method} {Path}", context.Request.Method, context.Request.Path);
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await WriteErrorAsync(context, "Internal Server Error", "An unexpected error occurred.");
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, string title, string detail)
    {
        context.Response.ContentType = "application/json";
        var error = new { title, status = context.Response.StatusCode, detail };
        await context.Response.WriteAsync(JsonSerializer.Serialize(error));
    }
}
