using System.Security.Cryptography;
using System.Text;

namespace Pulse.API.Infrastructure.Security;

public static class TokenHasher
{
    public static string Hash(string token)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = sha.ComputeHash(bytes);

        return Convert.ToBase64String(hash);
    }
}
