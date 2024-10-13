using AuthService.Configuration.Options;
using AuthService.Interfaces;
using Microsoft.Extensions.Options;

namespace AuthService.Services;


public class AdminTokenProvider : IAdminTokenProvider
{
    private readonly AdminTokenOptions _options;

    public AdminTokenProvider(IOptions<AdminTokenOptions> options)
    {
        _options = options.Value;
    }
    
    public Task<bool> ValidateAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return Task.FromResult(false);

        return Task.FromResult(_options.AdminToken == token);
    }
}