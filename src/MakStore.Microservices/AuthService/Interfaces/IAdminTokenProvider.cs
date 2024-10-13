namespace AuthService.Interfaces;

public interface IAdminTokenProvider
{
    Task<bool> ValidateAsync(string token);
}