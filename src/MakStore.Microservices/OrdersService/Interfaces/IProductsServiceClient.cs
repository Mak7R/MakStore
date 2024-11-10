namespace OrdersService.Interfaces;

public interface IProductsServiceClient
{
    Task<bool> CheckProductExists(Guid productId);
}