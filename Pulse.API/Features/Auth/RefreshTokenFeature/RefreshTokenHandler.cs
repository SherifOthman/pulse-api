using Pulse.API.Domain.Entities;
using Pulse.API.Features.Auth.LoginWithGoole;
using Pulse.API.Features.Auth.RefreshToken;
using Pulse.API.Infrastructure;
using Pulse.API.Infrastructure.Security;
using Pulse.API.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Pulse.API.Features.Auth.RefreshTokenFeature;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly AppDbContext _db;
    private readonly IJwtTokenGenerator _jwt;
    private readonly UserManager<ApplicationUser> _userManager;

    public RefreshTokenHandler(
        AppDbContext db,
        IJwtTokenGenerator jwt,
        UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _jwt = jwt;
        _userManager = userManager;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshTokenHash = TokenHasher.Hash(request.RefreshToken);

        var storedToken = await _db.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == refreshTokenHash, cancellationToken);

        if (storedToken == null)
            throw new Exception("Invalid refresh token");

        // Token was already rotated — this happens when the backend rotated the token
        // successfully but the response never reached the client (network error, etc.),
        // so the client still holds the old token. Follow the replacement chain once
        // and return the already-issued successor token as a new rotation.
        if (storedToken.RevokedAt != null && storedToken.ReplacedByTokenHash != null)
        {
            var successor = await _db.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.TokenHash == storedToken.ReplacedByTokenHash, cancellationToken);

            if (successor != null && successor.IsActive)
            {
                // Treat this as a fresh use of the successor token — rotate it now.
                storedToken = successor;
            }
            else
            {
                // Successor is also gone — potential token theft, reject.
                throw new Exception("Refresh token is not active");
            }
        }

        if (!storedToken.IsActive)
            throw new Exception("Refresh token is not active");

        var user = storedToken.User;

        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.RevokedByIp = request.IpAddress ?? "unknown";

        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = _jwt.Generate(user, roles);
        var newRawRefreshToken = RefreshTokenGenerator.GenerateToken();
        var newHash = TokenHasher.Hash(newRawRefreshToken);

        var newRefreshToken = new Domain.Entities.RefreshToken
        {
            TokenHash = newHash,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedByIp = request.IpAddress ?? "unknown"
        };

        _db.RefreshTokens.Add(newRefreshToken);
        storedToken.ReplacedByTokenHash = newHash;

        await _db.SaveChangesAsync(cancellationToken);

        return new AuthResponse(newAccessToken, newRawRefreshToken);
    }
}
