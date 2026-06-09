using Pulse.API.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using MediatR;

namespace Pulse.API.Features.Users.CreateUser;

public class CreateUserHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    : IRequestHandler<CreateUserCommand, UserResponse>
{
    public async Task<UserResponse> Handle(CreateUserCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            throw new BadHttpRequestException("Email and password are required");

        if (string.IsNullOrWhiteSpace(request.FullName))
            throw new BadHttpRequestException("Full name is required");

        var role = request.Role ?? "Manager";
        if (role != "Admin" && role != "Manager")
            throw new BadHttpRequestException("Role must be Admin or Manager");

        if (!await roleManager.RoleExistsAsync(role))
            throw new BadHttpRequestException($"Role '{role}' does not exist");

        var existing = await userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
            throw new BadHttpRequestException("Email already in use");

        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            FullName = request.FullName.Trim(),
            EmailConfirmed = true,
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new BadHttpRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

        await userManager.AddToRoleAsync(user, role);

        return new UserResponse(user.Id, user.Email, user.FullName, user.ImageUrl, user.EmailConfirmed, role);
    }
}
