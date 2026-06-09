using Pulse.API.Infrastructure;
using MediatR;

namespace Pulse.API.Features.Users.UpdateMe;

public class UpdateMeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/dashboard/users/me", async (HttpContext context, IMediator mediator, ICurrentUser currentUser) =>
        {
            var form = await context.Request.ReadFormAsync();
            var fullName = form["fullName"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(fullName))
                return Results.BadRequest(new { error = "FullName is required" });

            var image = form.Files.GetFile("image");

            var result = await mediator.Send(new UpdateMeCommand(currentUser.Id.ToString(), fullName, image));
            return Results.Ok(result);
        }).RequireAuthorization("ManagerOrAdmin").DisableAntiforgery();
    }
}
