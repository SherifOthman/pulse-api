using MediatR;

namespace Pulse.API.Features.Upload;

public class UploadEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/dashboard/upload", async (IFormFile file, IMediator mediator) =>
        {
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
        }).RequireAuthorization("ManagerOrAdmin");
    }
}
