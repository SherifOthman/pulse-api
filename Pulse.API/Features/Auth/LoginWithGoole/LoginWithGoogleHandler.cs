using Pulse.API.Domain.Entities;
using Pulse.API.Infrastructure;
using Pulse.API.Infrastructure.Google;
using Pulse.API.Infrastructure.Security;
using Pulse.API.Options;
using Pulse.API.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Pulse.API.Features.Auth.LoginWithGoole;

public class LoginWithGoogleHandler : IRequestHandler<LoginWithGoogleCommand, AuthResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly AppDbContext _db;
    private readonly IJwtTokenGenerator _jwt;
    private readonly IGoogleTokenValidator _googleTokenValidator;
    private readonly JWTOptions _options;

    private const string GoogleProvider = "Google";

    public LoginWithGoogleHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        AppDbContext db,
        IJwtTokenGenerator jwt,
        IGoogleTokenValidator googleTokenValidator,
        IOptions<JWTOptions> options)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _db = db;
        _jwt = jwt;
        _googleTokenValidator = googleTokenValidator;
        _options = options.Value;
    }

    public async Task<AuthResponse> Handle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
    {
        var payload = await _googleTokenValidator.ValidateAsync(request.IdToken);

        var providerKey = payload.Subject;
        var email = payload.Email;
        var name = payload.Name;
        var picture = payload.Picture;

        var user = await _userManager.FindByLoginAsync(GoogleProvider, providerKey);

        if (user != null)
        {
            // Eagerly load refresh tokens
            await _db.Entry(user).Collection(u => u.RefreshTokens).LoadAsync(cancellationToken);
            await _signInManager.ExternalLoginSignInAsync(GoogleProvider, providerKey, isPersistent: false);
        }
        else
        {
            var existingUserByEmail = await _userManager.FindByEmailAsync(email);

            if (existingUserByEmail != null)
            {
                await _db.Entry(existingUserByEmail).Collection(u => u.RefreshTokens).LoadAsync(cancellationToken);

                var addLoginResult = await _userManager.AddLoginAsync(
                    existingUserByEmail,
                    new UserLoginInfo(GoogleProvider, providerKey, GoogleProvider));

                if (!addLoginResult.Succeeded)
                {
                    throw new Exception($"Failed to link Google account: {string.Join(", ", addLoginResult.Errors.Select(e => e.Description))}");
                }

                user = existingUserByEmail;
            }
            else
            {
                user = new ApplicationUser
                {
                    Email = email,
                    UserName = email,
                    FullName = name ?? email,
                    EmailConfirmed = true,
                    ImageUrl = picture
                };

                var createResult = await _userManager.CreateAsync(user);

                if (!createResult.Succeeded)
                {
                    throw new Exception($"Failed to create user: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                }

                var addLoginResult = await _userManager.AddLoginAsync(
                    user,
                    new UserLoginInfo(GoogleProvider, providerKey, GoogleProvider));

                if (!addLoginResult.Succeeded)
                {
                    throw new Exception($"Failed to add Google login: {string.Join(", ", addLoginResult.Errors.Select(e => e.Description))}");
                }
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
        }

        if (user.FullName != (name ?? email) || user.ImageUrl != picture)
        {
            user.FullName = name ?? user.FullName;
            user.ImageUrl = picture ?? user.ImageUrl;
            await _userManager.UpdateAsync(user);
        }

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