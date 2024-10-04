using Asp.Versioning;
using AuthService.Identity;
using AuthService.Interfaces;
using AuthService.Models.Auth;
using MakStore.SharedComponents.Api;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace AuthService.Controllers;

[ApiVersion("1.0")]
public class AuthController : ApiController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthTokenService<ApplicationUser> _tokenService;

    public AuthController(UserManager<ApplicationUser> userManager, IAuthTokenService<ApplicationUser> tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginViewModel viewModel)
    {
        var user = await _userManager.FindByNameAsync(viewModel.UserName);
        if (user == null)
        {
            ModelState.AddModelError(nameof(LoginViewModel.UserName), "User with this username was not found");
            return ValidationProblem(ModelState);
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, viewModel.Password);
        if (!isPasswordValid)
        {
            ModelState.AddModelError(nameof(LoginViewModel.UserName), "Invalid username or password");
            ModelState.AddModelError(nameof(LoginViewModel.Password), "Invalid username or password");
            return ValidationProblem(ModelState);
        }

        var accessToken = await _tokenService.GetAccessTokenAsync(user);
        var refreshToken = await _tokenService.SaveRefreshTokenAsync(user);
        
        return Ok(new
        {
            UserId = user.Id,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterViewModel viewModel)
    {
        var user = new ApplicationUser
        {
            UserName = viewModel.UserName,
            Email = viewModel.Email
        };

        var result = await _userManager.CreateAsync(user, viewModel.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("Model", error.Description);
            }

            return ValidationProblem(ModelState);
        }

        return Ok(user.Id);
    }

    [HttpGet("validate")]
    public async Task<IActionResult> Validate([FromQuery] ValidateTokenRequestViewModel viewModel)
    {
        return Ok(await _tokenService.ValidateAccessTokenAsync(viewModel.Token));
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequestViewModel viewModel)
    {
        var user = await _userManager.FindByIdAsync(viewModel.UserId.ToString());
        if (user == null)
        {
            ModelState.AddModelError(nameof(viewModel.UserId), "User with this id was not found");
            return ValidationProblem(ModelState);
        }

        var isValid = await _tokenService.ValidateRefreshTokenAsync(user, viewModel.RefreshToken);
        if (!isValid)
        {
            ModelState.AddModelError(nameof(viewModel.RefreshToken), "Refresh token is not valid");
            return ValidationProblem(ModelState);
        }

        var refreshToken = await _tokenService.SaveRefreshTokenAsync(user);
        var accessToken = await _tokenService.GetAccessTokenAsync(user);

        return Ok(new
        {
            UserId = user.Id,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutViewModel viewModel)
    {
        var user = await _userManager.FindByIdAsync(viewModel.UserId.ToString());
        if (user == null)
        {
            ModelState.AddModelError(nameof(viewModel.UserId), "User with this id was not found");
            return ValidationProblem(ModelState);
        }

        await _tokenService.RemoveRefreshTokenAsync(user, viewModel.RefreshToken);
        return Ok(user.Id);
    }
}