using MediatR;

namespace Pulse.API.Features.Users.ListUsers;

public record ListUsersQuery : IRequest<List<UserResponse>>;
