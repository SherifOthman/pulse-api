namespace Pulse.API.Features.Auth.LoginWithGoole;

public record AuthResponse(
    string AccessToken,
    string RefreshToken
);
