using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeWebClient.Controllers;

[Controller]
[Route("account")]
public class AccountController : Controller
{
    public AccountController()
    {
    }
    
    [HttpGet("login")]
    public IActionResult Login(string? returnUrl)
    {
        var redirectUri = string.IsNullOrEmpty(returnUrl) ? "/" : new Uri(returnUrl).IsAbsoluteUri ? "/" : returnUrl;
        return Challenge(new AuthenticationProperties { RedirectUri = redirectUri }, "oidc");
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, "oidc");
    }
}