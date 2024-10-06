using System.Text;
using System.Text.Json;
using EmployeeWebClient.Configuration.Options;
using EmployeeWebClient.Models.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EmployeeWebClient.Services;

public class ProductsService : IProductsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ServicesOptions _options;

    public ProductsService(IHttpClientFactory httpClientFactory, IOptions<ServicesOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }

    public async Task<IEnumerable<ProductViewModel>> GetAll()
    {
        using var client = _httpClientFactory.CreateClient();
        var products = await client.GetFromJsonAsync<IEnumerable<ProductViewModel>>($"{_options.ProductsService.BaseUrl}/products") ?? [];
        return products;
    }

    public async Task<ProductViewModel?> GetById(Guid productId)
    {
        using var client = _httpClientFactory.CreateClient();
        var product =
            await client.GetFromJsonAsync<ProductViewModel>($"{_options.ProductsService.BaseUrl}/products/{productId}");
        return product;
    }

    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };
    public async Task<OperationResult<Guid>> Create(CreateProductViewModel product)
    {
        using var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.ProductsService.BaseUrl}/products");
        request.Content = new StringContent(
            JsonSerializer.Serialize(product),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.SendAsync(request);
        
        if (response.IsSuccessStatusCode)
        {
            var createdProductId = await response.Content.ReadAsStringAsync();
            return new OperationResult<Guid>
                { Result = Guid.Parse(createdProductId), StatusCode = (int)response.StatusCode };
        }
        
        var errorContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(errorContent, JsonSerializerOptions);
        return new OperationResult<Guid> { ProblemDetails = problemDetails, StatusCode = (int)response.StatusCode };
    }

    public async Task<OperationResult> Update(Guid productId, UpdateProductViewModel product)
    {
        using var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_options.ProductsService.BaseUrl}/products/{productId}");
        request.Content = new StringContent(
            JsonSerializer.Serialize(product),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.SendAsync(request);
        
        if (response.IsSuccessStatusCode)
            return new OperationResult { StatusCode = (int)response.StatusCode };
        
        var errorContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(errorContent, JsonSerializerOptions);
        return new OperationResult { ProblemDetails = problemDetails, StatusCode = (int)response.StatusCode };
    }

    public async Task<OperationResult> Delete(Guid productId, DeleteProductViewModel product)
    {
        using var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_options.ProductsService.BaseUrl}/products/{productId}");
        request.Content = new StringContent(
            JsonSerializer.Serialize(product),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.SendAsync(request);
        
        if (response.IsSuccessStatusCode)
            return new OperationResult { StatusCode = (int)response.StatusCode };
        
        var errorContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(errorContent, JsonSerializerOptions);
        return new OperationResult { ProblemDetails = problemDetails, StatusCode = (int)response.StatusCode };
    }
}