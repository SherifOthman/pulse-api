using Pulse.API.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using MediatR;

namespace Pulse.API.Features.Users.DeleteUser;

public class DeleteUserHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<DeleteUserCommand>
{
    private static readonly Guid AdminUserId = new("00000000-0000-0000-0000-0000000000AD");

    public async Task Handle(DeleteUserCommand request, CancellationToken ct)
    {
        if (request.Id == AdminUserId)
            throw new BadHttpRequestException("Cannot delete the system admin user");

        var user = await userManager.FindByIdAsync(request.Id.ToString());
        if (user is null)
            throw new KeyNotFoundException("User not found");

        var roles = await userManager.GetRolesAsync(user);
        if (roles.Contains("Admin"))
        {
            var adminCount = (await userManager.GetUsersInRoleAsync("Admin")).Count;
            if (adminCount <= 1)
                throw new BadHttpRequestException("Cannot delete the last admin user");
        }

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
            throw new BadHttpRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}
