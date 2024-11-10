namespace OrdersService.Interfaces;

public interface IUsersServiceClient
{
    Task<bool> CheckUserExists(Guid userId);
}