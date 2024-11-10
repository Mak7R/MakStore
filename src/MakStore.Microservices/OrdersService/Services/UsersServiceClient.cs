using OrdersService.Interfaces;

namespace OrdersService.Services;

public class UsersServiceClient : IUsersServiceClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public UsersServiceClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<bool> CheckUserExists(Guid userId)
    {
        return true;
    }
}