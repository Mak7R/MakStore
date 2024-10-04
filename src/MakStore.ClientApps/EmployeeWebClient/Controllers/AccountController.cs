using System.Text;
using System.Text.Json;
using EmployeeWebClient.Models.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
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

        // Подготовка запроса
        using var httpClient = _httpClientFactory.CreateClient();
    
        var requestContent = new StringContent(JsonSerializer.Serialize(new
        {
            loginViewModel.Username,
            loginViewModel.Password
        }), Encoding.UTF8, "application/json");

        // Отправка запроса на микросервис аутентификации
        var response = await httpClient.PostAsync("http://auth_service.makstore:8080/api/v1/login", requestContent);

        if (!response.IsSuccessStatusCode)
        {
            // Обработка ошибки авторизации
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(loginViewModel);
        }

        // Чтение данных из ответа
        var responseBody = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseBody);

        // Сохранение токенов в cookies
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // Включайте для HTTPS
            Expires = DateTime.UtcNow.AddDays(7) // Настройте срок действия cookie
        };

        // Сохранение AccessToken
        Response.Cookies.Append("AccessToken", authResponse.accessToken, cookieOptions);

        // Сохранение RefreshToken
        Response.Cookies.Append("RefreshToken", authResponse.refreshToken, cookieOptions);
        
        // Редирект или логика успешного входа
        return RedirectToAction("Index", "Home");
    }

    private record AuthResponse(string userId, string accessToken, string refreshToken);
}