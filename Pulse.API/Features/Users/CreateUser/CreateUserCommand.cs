using MediatR;

namespace Pulse.API.Features.Users.CreateUser;

public record CreateUserCommand(string Email, string Password, string FullName, string? Role) : IRequest<UserResponse>;
