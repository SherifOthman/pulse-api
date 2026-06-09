using Pulse.API.Domain.Entities;
using Pulse.API.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Pulse.API.Features.Users.GetMe;

public class GetMeHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<GetMeQuery, UserResponse>
{
    public async Task<UserResponse> Handle(GetMeQuery request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        if (user is null)
            throw new KeyNotFoundException("User not found");

        var roles = await userManager.GetRolesAsync(user);

        return new UserResponse(
            user.Id,
            user.Email!,
            user.FullName,
            user.ImageUrl,
            user.EmailConfirmed,
            roles.FirstOrDefault() ?? ""
        );
    }
}
