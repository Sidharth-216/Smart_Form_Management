using System.Net;
using System.Text.Json;

namespace API.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (InvalidOperationException ex)
        {
            await WriteProblemAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception)
        {
            await WriteProblemAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
        }
    }

    private static Task WriteProblemAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        var payload = JsonSerializer.Serialize(new { success = false, message });
        return context.Response.WriteAsync(payload);
    }
}
