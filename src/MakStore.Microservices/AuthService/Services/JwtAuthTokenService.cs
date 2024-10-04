using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Configuration.Options;
using AuthService.Identity;
using AuthService.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services;

public class JwtAuthTokenService : IAuthTokenService<ApplicationUser>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtTokenOptions _options;
    private static readonly List<(Guid UserId, string RefreshToken, DateTime Expiration)> TempUserTokenStorage = new();

    public JwtAuthTokenService(UserManager<ApplicationUser> userManager, IOptions<JwtTokenOptions> options)
    {
        _userManager = userManager;
        _options = options.Value;
    }

    public async Task<string> GetAccessTokenAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.AccessTokenExpirationMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Task<bool> ValidateAccessTokenAsync(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
                ValidateIssuer = !string.IsNullOrEmpty(_options.Issuer),
                ValidIssuer = _options.Issuer,
                ValidateAudience = !string.IsNullOrEmpty(_options.Audience),
                ValidAudience = _options.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<string> SaveRefreshTokenAsync(ApplicationUser user, string? currentRefreshToken = null)
    {
        DateTime expiration;
        if (!string.IsNullOrEmpty(currentRefreshToken))
        {
            var oldTokens = TempUserTokenStorage.FindAll(t => t.UserId == user.Id && t.RefreshToken == currentRefreshToken);
            expiration = oldTokens.Min(t => t.Expiration);
        }
        else
        {
            expiration = DateTime.UtcNow.AddDays(_options.RefreshTokenExpirationDays);
        }
        
        var refreshToken = Guid.NewGuid().ToString();
        TempUserTokenStorage.Add((user.Id, refreshToken, expiration));

        return Task.FromResult(refreshToken);
    }

    public Task<bool> ValidateRefreshTokenAsync(ApplicationUser user, string refreshToken)
    {
        var storedToken = TempUserTokenStorage.FirstOrDefault(t => t.UserId == user.Id && t.RefreshToken == refreshToken);
        if (storedToken != default && storedToken.Expiration > DateTime.UtcNow)
        {
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task RemoveRefreshTokenAsync(ApplicationUser user, string refreshToken)
    {
        TempUserTokenStorage.RemoveAll(t => t.UserId == user.Id && t.RefreshToken == refreshToken);
        return Task.CompletedTask;
    }
}