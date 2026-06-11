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

        int statusCode;
        string message;

        switch (ex)
        {
            case NotFoundException:
                statusCode = 404; message = ex.Message; break;
            case BadRequestException:
                statusCode = 400; message = ex.Message; break;
            case UnauthorizedException:
            case UnauthorizedAccessException:
                statusCode = 401; message = ex.Message; break;
            case ForbiddenException:
                statusCode = 403; message = ex.Message; break;
            case ConflictException:
                statusCode = 409; message = ex.Message; break;
            case KeyNotFoundException:
                statusCode = 404; message = ex.Message; break;
            default:
                statusCode = 500;
                message = isDev
                    ? $"{ex.GetType().Name}: {ex.Message}" +
                      (ex.InnerException != null ? $"\nInner: {ex.InnerException.Message}" : "")
                    : "حدث خطأ غير متوقع في الخادم";
                break;
        }

        context.Response.StatusCode      = statusCode;
        context.Response.ContentType     = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = message });
    }
}

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseAppExceptionMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionMiddleware>();
}
