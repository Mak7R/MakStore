using Asp.Versioning;
using AuthService.Configuration.Options;
using IdentityModel.Client;
using MakStore.SharedComponents.Api;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiVersion("1.0")]
public class DevToolsController : ApiController
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly DevToolsOptions? _devToolsOptions;

    public DevToolsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;


        var devToolsOptionsSection = configuration.GetSection("DevTools");
        
        var devToolsOptions = devToolsOptionsSection.Get<DevToolsOptions>();
        _devToolsOptions = devToolsOptions;
    }
    
    [HttpGet("dev-tools/get-access-token")]
    public async Task<IActionResult> GetAccessToken(string userName, string password)
    {
        if (_devToolsOptions is not { IsEnabled: true })
            return NotFound();
        
        var client = _httpClientFactory.CreateClient();

        var discoveryDocument = await client.GetDiscoveryDocumentAsync(_devToolsOptions.Authority);
        if (discoveryDocument.IsError)
        {
            return BadRequest("Error retrieving discovery document: " + discoveryDocument.Error);
        }
        
        var tokenRequest = new PasswordTokenRequest
        {
            Address = discoveryDocument.TokenEndpoint,
            ClientId = _devToolsOptions.ClientId,
            ClientSecret = _devToolsOptions.ClientSecret,
            UserName = userName,
            Password = password,
            Scope = _devToolsOptions.Scope
        };
        
        var tokenResponse = await client.RequestPasswordTokenAsync(tokenRequest);

        if (tokenResponse.IsError)
        {
            return BadRequest("Error retrieving access token: " + tokenResponse.Error);
        }
        
        return Ok(new
        {
            tokenResponse.AccessToken,
            tokenResponse.ExpiresIn
        });
    }
}