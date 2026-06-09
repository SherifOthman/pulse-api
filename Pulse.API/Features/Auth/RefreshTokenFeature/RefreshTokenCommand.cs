using Pulse.API.Features.Auth.LoginWithGoole;
using MediatR;

namespace Pulse.API.Features.Auth.RefreshToken;

public record RefreshTokenCommand(
    string AccessToken,
    string RefreshToken,
    string? IpAddress): IRequest<AuthResponse>;
