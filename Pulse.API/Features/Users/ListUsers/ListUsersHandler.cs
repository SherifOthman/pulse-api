using Pulse.API.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace Pulse.API.Features.Users.ListUsers;

public class ListUsersHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<ListUsersQuery, List<UserResponse>>
{
    public async Task<List<UserResponse>> Handle(ListUsersQuery request, CancellationToken ct)
    {
        var users = await userManager.Users.ToListAsync(ct);
        var result = new List<UserResponse>(users.Count);

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            result.Add(new UserResponse(
                user.Id,
                user.Email!,
                user.FullName,
                user.ImageUrl,
                user.EmailConfirmed,
                roles.FirstOrDefault() ?? ""
            ));
        }

        return result;
    }
}
