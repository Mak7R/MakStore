namespace AuthService.Configuration.Options;

public class DevToolsOptions
{
    public bool IsEnabled { get; set; }
    public string Authority { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string? Scope { get; set; }
}