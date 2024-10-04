using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace MakStore.SharedComponents.Authentication.Options;

public class MicroservicesAuthenticationOptions : AuthenticationSchemeOptions
{
    public string ValidateAccessTokenUrl { get; set; }
    
    public Func<HttpContext, string?> AccessTokenProvider { get; set; }
    public Func<HttpContext, string?> RefreshTokenProvider { get; set; }
}