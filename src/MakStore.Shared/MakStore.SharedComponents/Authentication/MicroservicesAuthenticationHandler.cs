using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using MakStore.SharedComponents.Authentication.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MakStore.SharedComponents.Authentication;

public class MicroservicesAuthenticationHandler : AuthenticationHandler<MicroservicesAuthenticationOptions>
{
    private readonly IHttpClientFactory _httpClientFactory;

    public MicroservicesAuthenticationHandler(
        IOptionsMonitor<MicroservicesAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IHttpClientFactory httpClientFactory)
        : base(options, logger, encoder)
    {
        _httpClientFactory = httpClientFactory;
    }

    protected override object? Events { get; set; }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var accessToken = Options.AccessTokenProvider(Context);
        if (string.IsNullOrEmpty(accessToken))
        {
            return AuthenticateResult.NoResult();
        }

        using var httpClient = _httpClientFactory.CreateClient();
        
        var response = await httpClient.GetAsync(Options.ValidateAccessTokenUrl + $"?token={accessToken}");
        var responseContent = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode || !responseContent.Equals("true"))
        {
            return AuthenticateResult.Fail("Access token is invalid");
        }
        
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(accessToken);
        
        var identity = new ClaimsIdentity(jwtToken.Claims.ToList(), Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}