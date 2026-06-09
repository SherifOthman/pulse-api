using MediatR;

namespace Pulse.API.Features.Users.GetMe;

public record GetMeQuery(string UserId) : IRequest<UserResponse>;
