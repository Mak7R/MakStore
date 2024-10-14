using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.Account;

public class RegisterAdminViewModel
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string AdminToken { get; set; } = string.Empty;
    
    public string? ReturnUrl { get; set; }
}