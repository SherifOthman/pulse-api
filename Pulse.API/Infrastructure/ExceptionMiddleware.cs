using FluentValidation;
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
        object response;

        switch (ex)
        {
            case NotFoundException:
                statusCode = 404;
                response = new { error = ex.Message };
                break;
            case BadRequestException:
                statusCode = 400;
                response = new { error = ex.Message };
                break;
            case UnauthorizedException:
            case UnauthorizedAccessException:
                statusCode = 401;
                response = new { error = ex.Message };
                break;
            case ForbiddenException:
                statusCode = 403;
                response = new { error = ex.Message };
                break;
            case ConflictException:
                statusCode = 409;
                response = new { error = ex.Message };
                break;
            case KeyNotFoundException:
                statusCode = 404;
                response = new { error = ex.Message };
                break;
            case ValidationException vex:
                statusCode = 400;
                response = new
                {
                    error = "Validation failed",
                    errors = vex.Errors.GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                };
                break;
            default:
                statusCode = 500;
                var message = isDev
                    ? $"{ex.GetType().Name}: {ex.Message}" +
                      (ex.InnerException != null ? $"\nInner: {ex.InnerException.Message}" : "")
                    : "حدث خطأ غير متوقع في الخادم";
                response = new { error = message };
                break;
        }

        context.Response.StatusCode      = statusCode;
        context.Response.ContentType     = "application/json";
        await context.Response.WriteAsJsonAsync(response);
    }
}

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseAppExceptionMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionMiddleware>();
}
