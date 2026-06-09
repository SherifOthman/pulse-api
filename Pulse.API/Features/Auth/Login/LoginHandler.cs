using Pulse.API.Domain.Entities;
using Pulse.API.Features.Auth.LoginWithGoole;
using Pulse.API.Infrastructure;
using Pulse.API.Infrastructure.Security;
using Pulse.API.Options;
using Pulse.API.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Pulse.API.Features.Auth.Login;

public class LoginHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly AppDbContext _db;
    private readonly IJwtTokenGenerator _jwt;
    private readonly JWTOptions _options;

    public LoginHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        AppDbContext db,
        IJwtTokenGenerator jwt,
        IOptions<JWTOptions> options)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _db = db;
        _jwt = jwt;
        _options = options.Value;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        if (!result.Succeeded)
            throw new UnauthorizedAccessException("Invalid credentials.");

        // Load refresh tokens
        await _db.Entry(user).Collection(u => u.RefreshTokens).LoadAsync(cancellationToken);

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwt.Generate(user, roles);
        var rawRefreshToken = RefreshTokenGenerator.GenerateToken();
        var hashedRefreshToken = TokenHasher.Hash(rawRefreshToken);

        var existingRefreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.IsActive);
        if (existingRefreshToken != null)
        {
            existingRefreshToken.RevokedAt = DateTime.UtcNow;
            existingRefreshToken.RevokedByIp = request.IpAddress ?? "system";
            existingRefreshToken.ReplacedByTokenHash = hashedRefreshToken;
        }

        var refreshToken = new Domain.Entities.RefreshToken
        {
            TokenHash = hashedRefreshToken,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_options.RefreshTokenExpireDays),
            CreatedByIp = request.IpAddress ?? "unknown"
        };

        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync(cancellationToken);

        return new AuthResponse(accessToken, rawRefreshToken);
    }
}
