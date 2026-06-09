using Pulse.API.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Pulse.API.Features.Users.ChangePassword;

public class ChangePasswordHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<ChangePasswordCommand, ChangePasswordResponse>
{
    public async Task<ChangePasswordResponse> Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
            throw new BadHttpRequestException("Current password and new password are required");

        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            throw new KeyNotFoundException("User not found");

        var check = await userManager.CheckPasswordAsync(user, request.CurrentPassword);
        if (!check)
            throw new BadHttpRequestException("Current password is incorrect");

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, request.NewPassword);
        if (!result.Succeeded)
            throw new BadHttpRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

        return new ChangePasswordResponse("Password changed successfully");
    }
}
