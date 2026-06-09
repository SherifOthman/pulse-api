using MediatR;

namespace Pulse.API.Features.Users.UpdateMe;

public record UpdateMeCommand(
    string UserId,
    string FullName,
    IFormFile? Image) : IRequest<UserResponse>;
