namespace AuthService.Interfaces;

public interface IRefreshTokenProvider
{
    Task<bool> ValidateAsync(Guid userId, string refreshToken);
    Task<string> CreateAsync(Guid userId);
    Task<string> UpdateAsync(Guid userId, string refreshToken);
    Task RemoveAsync(Guid userId, string refreshToken);
}