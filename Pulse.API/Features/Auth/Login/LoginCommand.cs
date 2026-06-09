using Pulse.API.Features.Auth.LoginWithGoole;
using MediatR;

namespace Pulse.API.Features.Auth.Login;

public record LoginCommand(
    string Email,
    string Password,
    string? IpAddress) : IRequest<AuthResponse>;
