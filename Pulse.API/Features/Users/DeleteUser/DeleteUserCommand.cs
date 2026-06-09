using MediatR;

namespace Pulse.API.Features.Users.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest;
