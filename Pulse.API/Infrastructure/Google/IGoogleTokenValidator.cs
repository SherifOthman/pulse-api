using Google.Apis.Auth;

namespace Pulse.API.Infrastructure.Google;

public interface IGoogleTokenValidator
{
    Task<GoogleJsonWebSignature.Payload> ValidateAsync(string idToken);

}
