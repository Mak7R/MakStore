namespace AuthService.Configuration.Options;

public class JwtTokenOptions
{
    public string SecretKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public double AccessTokenExpirationMinutes { get; set; }
    public double RefreshTokenExpirationDays { get; set; }
}