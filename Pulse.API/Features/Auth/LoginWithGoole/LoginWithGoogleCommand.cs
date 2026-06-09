using MediatR;

namespace Pulse.API.Features.Auth.LoginWithGoole;

public record LoginWithGoogleCommand(
    string IdToken,
    string? IpAddress) : IRequest<AuthResponse>;
