using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace EmployeeWebClient.Auth;

public static class MicroservicesAuthenticationDefaults
{
    public const string AuthenticationScheme = "MicroservicesAuthScheme";
}

public class MicroservicesAuthenticationOptions : AuthenticationSchemeOptions
{
    public string AccessTokenCookie { get; set; } = "AccessToken";
    public string RefreshTokenCookie { get; set; } = "RefreshToken";
    public string ValidateTokenUrl { get; set; }
}

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

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var accessToken = Request.Cookies[Options.AccessTokenCookie];
        if (string.IsNullOrEmpty(accessToken))
        {
            return AuthenticateResult.NoResult();
        }

        using var httpClient = _httpClientFactory.CreateClient();
        
        var response = await httpClient.GetAsync(Options.ValidateTokenUrl + "?token={accessToken}");
        var responseContent = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode || !responseContent.Equals("true"))
        {
            return AuthenticateResult.Fail("Access token is invalid");
        }
        
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(accessToken);
        var claims = jwtToken.Claims.ToList(); 
        
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}