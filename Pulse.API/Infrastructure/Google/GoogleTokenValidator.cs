using Pulse.API.Options;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;

namespace Pulse.API.Infrastructure.Google;

public class GoogleTokenValidator : IGoogleTokenValidator
{
    private readonly GoogleAuthOptions _options;
    public GoogleTokenValidator(IOptions<GoogleAuthOptions> options)
    {
        _options = options.Value;
    }


    public async Task<GoogleJsonWebSignature.Payload> ValidateAsync(string idToken)
    {
        var audiences = new List<string> { _options.WebClientId };

        if (!string.IsNullOrEmpty(_options.AndroidClientId))
            audiences.Add(_options.AndroidClientId);

        if (!string.IsNullOrEmpty(_options.IosClientId))
            audiences.Add(_options.IosClientId);

        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = audiences
        };

        return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
    }
}
