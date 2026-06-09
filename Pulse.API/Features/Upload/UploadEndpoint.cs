using MediatR;

namespace Pulse.API.Features.Upload;

public class UploadEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/dashboard/upload", async (HttpContext httpContext, IMediator mediator) =>
        {
            IFormFile? file;
            try
            {
                if (!httpContext.Request.HasFormContentType)
                    return Results.BadRequest(new { message = "No file provided" });

                var form = await httpContext.Request.ReadFormAsync();
                file = form.Files.GetFile("file");
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = $"Failed to read form: {ex.Message}" });
            }

            if (file is null || file.Length == 0)
                return Results.BadRequest(new { message = "No file provided" });

            try
            {
                var url = await mediator.Send(new UploadCommand(file));
                return Results.Ok(new { url });
            }
            catch (BadHttpRequestException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(detail: ex.Message, statusCode: 500);
            }
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
