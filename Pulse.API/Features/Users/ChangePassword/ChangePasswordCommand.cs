using MediatR;

namespace Pulse.API.Features.Users.ChangePassword;

public record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword,
    Guid UserId
) : IRequest<ChangePasswordResponse>;
