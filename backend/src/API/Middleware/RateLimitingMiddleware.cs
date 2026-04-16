using Application.Contracts;

namespace API.Middleware;

public sealed class RateLimitingMiddleware(RequestDelegate next)
{
    private const int LimitPerMinute = 100;

    public async Task InvokeAsync(HttpContext context, ICacheService cache)
    {
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await next(context);
            return;
        }

        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var key = $"rate:{ip}:{DateTime.UtcNow:yyyyMMddHHmm}";
        var count = await cache.IncrementAsync(key, TimeSpan.FromMinutes(1));

        if (count > LimitPerMinute)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsJsonAsync(new { success = false, message = "Rate limit exceeded. Try again later." });
            return;
        }

        await next(context);
    }
}
