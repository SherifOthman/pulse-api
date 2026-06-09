using System.Security.Cryptography;

namespace Pulse.API.Infrastructure;

public static class RefreshTokenGenerator
{

    public static string GenerateToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

}
