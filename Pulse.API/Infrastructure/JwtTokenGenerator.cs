using Pulse.API.Domain.Entities;
using Pulse.API.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;

namespace Pulse.API.Infrastructure;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JWTOptions _options;

    public JwtTokenGenerator(IOptions<JWTOptions> jwtOptions)
    {
        _options = jwtOptions.Value;
    }

    public string Generate(ApplicationUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Adudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpireMinutes),
            signingCredentials:creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
