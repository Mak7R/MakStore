using AuthService.Identity;
using AuthService.Models.Account;
using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[Route("account")]
public class AccountController : Controller
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(
        IIdentityServerInteractionService interaction, 
        UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager)
    {
        _interaction = interaction;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet("/")]
    public IActionResult Index()
    {
        return RedirectToAction("Login");
    }
    
    [HttpGet("login")]
    public IActionResult Login(string? returnUrl)
    {
        return View(new LoginViewModel{ReturnUrl = returnUrl});
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
        
        var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, true, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            return Redirect(model.ReturnUrl ?? "/");
        }

        if (result.IsNotAllowed)
        {
            ModelState.AddModelError(nameof(LoginViewModel.Username), "Log in is not allowed");
            ModelState.AddModelError(nameof(LoginViewModel.Password), "Log in is not allowed");
        }
        else
        {
            ModelState.AddModelError(nameof(LoginViewModel), "Log in failure");
        }
        
        return View(model);
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout(string? logoutId)
    {
        var logout = await _interaction.GetLogoutContextAsync(logoutId);
        
        if (User.Identity?.IsAuthenticated == true)
        {
            await _signInManager.SignOutAsync();

            // see if we need to trigger federated logout
            var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

            // if it's a local login we can ignore this workflow
            if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
            {
                // we need to see if the provider supports external logout
                var provider = HttpContext.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
                var handler = await provider.GetHandlerAsync(HttpContext, idp);
                if (handler is IAuthenticationSignOutHandler)
                {
                    // this triggers a redirect to the external provider for sign-out
                    return SignOut(new AuthenticationProperties { RedirectUri = logout.PostLogoutRedirectUri }, idp);
                }
            }
        }
        
        if (string.IsNullOrEmpty(logout.PostLogoutRedirectUri))
            return LocalRedirect("/");
        
        return Redirect(logout.PostLogoutRedirectUri);
    }

    [HttpGet("register")]
    public IActionResult RegisterClient(string? returnUrl)
    {
        return View(new RegisterViewModel{ReturnUrl = returnUrl});
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> RegisterClient(RegisterViewModel model)
    {
        var user = new ApplicationUser
        {
            UserName = model.Username,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("Model", error.Description);
            }

            return View(model);
        }

        await _userManager.AddToRoleAsync(user, nameof(UserRole.Client));
        await _signInManager.SignInAsync(user, true);

        return Redirect(model.ReturnUrl ?? "/");
    }
    
    // todo: read user, update user, update user password, delete user etc 
}