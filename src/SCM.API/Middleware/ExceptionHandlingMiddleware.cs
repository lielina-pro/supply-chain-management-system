using System.Net;
using System.Text.Json;
using SCM.Application.Common.Exceptions;

namespace SCM.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _log;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> log)
        => (_next, _log) = (next, log);

    public async Task InvokeAsync(HttpContext ctx)
    {
        try { await _next(ctx); }
        catch (NotFoundException ex)
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await WriteJson(ctx, ex.Message);
        }
        catch (BusinessRuleException ex)
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
            await WriteJson(ctx, ex.Message);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Unhandled exception");
            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await WriteJson(ctx, "An unexpected error occurred.");
        }
    }

    private static Task WriteJson(HttpContext ctx, string message)
    {
        ctx.Response.ContentType = "application/json";
        return ctx.Response.WriteAsync(JsonSerializer.Serialize(new { error = message }));
    }
}
