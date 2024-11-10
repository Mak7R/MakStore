using MakStore.SharedComponents.Configuration;
using MakStore.SharedComponents.Constants;
using Microsoft.Extensions.Options;
using OrdersService.Interfaces;

namespace OrdersService.Services;

public class ProductsServiceClient : IProductsServiceClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ServicesOptions _options;

    public ProductsServiceClient(IHttpClientFactory httpClientFactory, IOptions<ServicesOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }
    public async Task<bool> CheckProductExists(Guid productId)
    {
        using var http = _httpClientFactory.CreateClient(AutoAuthHttpClientDefaults.ClientName);

        var response = await http.GetAsync($"{_options.ProductsService.BaseUrl}/api/v1/products/{productId}");
        
        return response.IsSuccessStatusCode;
    }
}