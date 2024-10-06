using EmployeeWebClient.Models.Product;

namespace EmployeeWebClient.Services;

public interface IProductsService
{
    Task<IEnumerable<ProductViewModel>> GetAll();
    Task<ProductViewModel?> GetById(Guid productId);

    Task<OperationResult<Guid>> Create(CreateProductViewModel product);
    Task<OperationResult> Update(Guid productId, UpdateProductViewModel product);
    Task<OperationResult> Delete(Guid productId, DeleteProductViewModel product);
}