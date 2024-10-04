using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.Auth;

public class ValidateTokenRequestViewModel
{
    [Required]
    public string Token { get; set; } = string.Empty;
}