using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.Auth;

public class RefreshTokenRequestViewModel
{
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}