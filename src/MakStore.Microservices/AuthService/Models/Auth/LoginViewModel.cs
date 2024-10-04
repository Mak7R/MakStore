using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.Auth;

public class LoginViewModel
{
    [Required]
    public string UserName { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
    
}