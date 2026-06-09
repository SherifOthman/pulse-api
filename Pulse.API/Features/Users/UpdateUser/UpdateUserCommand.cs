using MediatR;

namespace Pulse.API.Features.Users.UpdateUser;

public record UpdateUserCommand(Guid Id, string? FullName, string? Email, string? Password, string? Role) : IRequest<UserResponse>;
