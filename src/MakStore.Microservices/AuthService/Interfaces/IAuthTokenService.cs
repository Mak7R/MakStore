using AuthService.Identity;

namespace AuthService.Interfaces;

public interface IAuthTokenService<TUser>
{
    public Task<string> GetAccessTokenAsync(TUser user);

    public Task<bool> ValidateAccessTokenAsync(string token);
    
    public Task<string> SaveRefreshTokenAsync(TUser user, string? currentRefreshToken = null);
    public Task<bool> ValidateRefreshTokenAsync(TUser user, string refreshToken);
    public Task RemoveRefreshTokenAsync(TUser user, string refreshToken);
}