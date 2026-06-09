using Pulse.API.Domain.Entities;
using Pulse.API.Infrastructure.Exceptions;
using Pulse.API.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Pulse.API.Features.Users.UpdateMe;

public class UpdateMeHandler(
    UserManager<ApplicationUser> userManager,
    IFileService fileService)
    : IRequestHandler<UpdateMeCommand, UserResponse>
{
    public async Task<UserResponse> Handle(UpdateMeCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        if (user is null)
            throw new KeyNotFoundException("User not found");

        user.FullName = request.FullName;

        if (request.Image is not null)
        {
            if (user.ImageUrl is not null)
                await fileService.DeleteFileAsync(user.ImageUrl);
            user.ImageUrl = await fileService.SaveProfileImageAsync(request.Image);
        }

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

        var roles = await userManager.GetRolesAsync(user);
        return new UserResponse(user.Id, user.Email!, user.FullName, user.ImageUrl, user.EmailConfirmed, roles.FirstOrDefault() ?? "");
    }
}
