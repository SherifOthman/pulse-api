using Pulse.API.Infrastructure.Exceptions;

namespace Pulse.API.Infrastructure;

public class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var isDev = context.RequestServices
            .GetRequiredService<IWebHostEnvironment>()
            .IsDevelopment();

        var (statusCode, message) = ex switch
        {
            NotFoundException        => (404, ex.Message),
            BadRequestException      => (400, ex.Message),
            UnauthorizedException or
                UnauthorizedAccessException => (401, ex.Message),
            ForbiddenException       => (403, ex.Message),
            ConflictException        => (409, ex.Message),
            KeyNotFoundException     => (404, ex.Message),
            _                        => (500, isDev
                ? $"{ex.GetType().Name}: {ex.Message}\n{ex.InnerException?.Message}"
                : "An unexpected error occurred")
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = message });
    }
}

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseAppExceptionMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionMiddleware>();
}
