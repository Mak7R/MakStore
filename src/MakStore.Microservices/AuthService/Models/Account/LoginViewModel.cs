using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.Account;

public class LoginViewModel
{
    [Required]
    public string Username { get; set; }
    
    [Required]
    public string Password { get; set; }

    public string? ReturnUrl { get; set; } = "/";
}