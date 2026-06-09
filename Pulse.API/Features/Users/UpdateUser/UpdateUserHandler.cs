using Pulse.API.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using MediatR;

namespace Pulse.API.Features.Users.UpdateUser;

public class UpdateUserHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    : IRequestHandler<UpdateUserCommand, UserResponse>
{
    public async Task<UserResponse> Handle(UpdateUserCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.Id.ToString());
        if (user is null)
            throw new KeyNotFoundException("User not found");

        if (request.FullName is not null)
            user.FullName = string.IsNullOrWhiteSpace(request.FullName) ? user.FullName : request.FullName.Trim();

        if (request.Email is not null)
        {
            var email = request.Email.Trim();
            var existing = await userManager.FindByEmailAsync(email);
            if (existing is not null && existing.Id != user.Id)
                throw new BadHttpRequestException("Email already in use");
            user.Email = email;
            user.UserName = email;
        }

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new BadHttpRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

        if (request.Password is not null)
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var pwResult = await userManager.ResetPasswordAsync(user, token, request.Password);
            if (!pwResult.Succeeded)
                throw new BadHttpRequestException(string.Join(", ", pwResult.Errors.Select(e => e.Description)));
        }

        if (request.Role is not null)
        {
            if (request.Role != "Admin" && request.Role != "Manager")
                throw new BadHttpRequestException("Role must be Admin or Manager");

            if (!await roleManager.RoleExistsAsync(request.Role))
                throw new BadHttpRequestException($"Role '{request.Role}' does not exist");

            var currentRoles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, currentRoles);
            await userManager.AddToRoleAsync(user, request.Role);
        }

        var roles = await userManager.GetRolesAsync(user);

        return new UserResponse(user.Id, user.Email!, user.FullName, user.ImageUrl, user.EmailConfirmed, roles.FirstOrDefault() ?? "");
    }
}
