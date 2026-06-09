namespace Pulse.API.Features.Users;

public record UserResponse(
    Guid Id,
    string Email,
    string FullName,
    string? ImageUrl,
    bool EmailConfirmed,
    string Role
);
