using System.ComponentModel.DataAnnotations;

namespace EmployeeWebClient.Models.Account;

public class LoginViewModel
{
    [Required]
    public string Username { get; set; }
    
    [Required]
    public string Password { get; set; }
}