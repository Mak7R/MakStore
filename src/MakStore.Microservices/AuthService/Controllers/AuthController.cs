using Asp.Versioning;
using AuthService.Identity;
using AuthService.Models.Account;
using MakStore.Domain.Enums;
using MakStore.SharedComponents.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace AuthService.Controllers;

[ApiVersion("1.0")]
public class AuthController : ApiController
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    
    [HttpPost("register/admin")]
    public async Task<IActionResult> RegisterAdmin(RegisterViewModel viewModel, string adminToken)
    {
        // todo validateAdminToken
        
        var user = new ApplicationUser
        {
            UserName = viewModel.Username,
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

        await _userManager.AddToRoleAsync(user, nameof(UserRole.Admin));
        
        return Ok(user.Id);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost("register/employee")]
    public async Task<IActionResult> RegisterEmployee(RegisterViewModel viewModel)
    {
        var user = new ApplicationUser
        {
            UserName = viewModel.Username,
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

        await _userManager.AddToRoleAsync(user, nameof(UserRole.Employee));
        
        return Ok(user.Id);
    }
}