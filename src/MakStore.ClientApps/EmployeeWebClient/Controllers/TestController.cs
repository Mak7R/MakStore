using EmployeeWebClient.Models.Product;
using IdentityModel.Client;
using MakStore.SharedComponents.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EmployeeWebClient.Controllers;

[Controller]
[Route("test")]
public class TestController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TestController> _logger;
    private readonly ServicesOptions _options;

    public TestController(IHttpClientFactory httpClientFactory, ILogger<TestController> logger, IOptions<ServicesOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _options = options.Value;
    }
    
    [HttpGet("identity")]
    public async Task<IActionResult> TestIdentity()
    {
        using var client = _httpClientFactory.CreateClient();
        var disco = await client.GetDiscoveryDocumentAsync("https://host.docker.internal:9011");
        
        _logger.LogInformation("DERR: {err}", disco.Error);
        
        if (disco.IsError)
        {
            _logger.LogError(disco.Exception, disco.Error);
            return StatusCode(500);
        }

        var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = "client",
            ClientSecret = "secret",
            Scope = "products_api"
        });

        _logger.LogInformation("RERR: {err}", tokenResponse.Error);
        
        if (tokenResponse.IsError)
        {
            _logger.LogError(tokenResponse.Exception, tokenResponse.Error);
            return StatusCode(500);
        }

        try
        {
            using var apiClient = _httpClientFactory.CreateClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken!);
            var products =
                await apiClient.GetFromJsonAsync<IEnumerable<ProductViewModel>>(
                    $"{_options.ProductsService.BaseUrl}/api/v1/products");

            return Ok(products);
        }
        catch (Exception e)
        {
            return Ok(e);
        }
    }
}