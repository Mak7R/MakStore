namespace MakStore.SharedComponents.Mediator;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
    void Rollback();
}