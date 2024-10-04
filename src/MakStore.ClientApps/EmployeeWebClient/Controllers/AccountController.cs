using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using EmployeeWebClient.Models.Account;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeWebClient.Controllers;

[Controller]
[Route("account")]
public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AccountController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    [HttpGet("login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        if (!ModelState.IsValid)
            return View(loginViewModel);
        
        using var httpClient = _httpClientFactory.CreateClient();
    
        var requestContent = new StringContent(JsonSerializer.Serialize(new
        {
            loginViewModel.Username,
            loginViewModel.Password
        }), Encoding.UTF8, "application/json");
        
        var response = await httpClient.PostAsync("http://auth_service.makstore:8080/api/v1/login", requestContent);

        if (!response.IsSuccessStatusCode)
        {
            // Обработка ошибки авторизации
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(loginViewModel);
        }
        
        var responseBody = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseBody);
        
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        
        Response.Cookies.Append("AccessToken", authResponse.AccessToken, cookieOptions);
        Response.Cookies.Append("RefreshToken", authResponse.RefreshToken, cookieOptions);
        
        return RedirectToAction("Index", "Home");
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("AccessToken");
        Response.Cookies.Delete("RefreshToken");
        
        return RedirectToAction("Index", "Home");
    }
    
    
    private record AuthResponse(
        [property: JsonPropertyName("userId")] string UserId,
        [property: JsonPropertyName("accessToken")] string AccessToken, 
        [property: JsonPropertyName("refreshToken")] string RefreshToken
        );
}