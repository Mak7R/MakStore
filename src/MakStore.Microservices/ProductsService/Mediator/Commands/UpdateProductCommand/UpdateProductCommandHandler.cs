using FluentValidation;
using MakStore.SharedComponents.Exceptions;
using MediatR;
using ProductsService.Infrastructure.Repositories;
using ProductsService.Mapping;
using ProductsService.Models;
using ValidationException = MakStore.SharedComponents.Exceptions.ValidationException;

namespace ProductsService.Mediator.Commands.UpdateProductCommand;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly IProductsRepository _productsRepository;
    private readonly IValidator<UpdateProductCommand> _validator;

    public UpdateProductCommandHandler(IProductsRepository productsRepository, IValidator<UpdateProductCommand> validator)
    {
        _productsRepository = productsRepository;
        _validator = validator;
    }
    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.ToDictionary(),  $"{nameof(UpdateProductCommand)} validation failed");
        
        var existsProduct = await _productsRepository.FindByIdAsync(request.Id, cancellationToken);
        if (existsProduct == null)
            throw new NotFoundException($"Product with id '{request.Id}' was not found");
        
        var sameNameProduct = await _productsRepository.FindByNameAsync(request.Name, cancellationToken);
        if (sameNameProduct != null && sameNameProduct.Id != existsProduct.Id)
            throw new AlreadyExistsException($"Product with name '{request.Name}' already exists");
        
        var product = ApplicationMapper.Mapper.Map<Product>(request);
        
        await _productsRepository.UpdateAsync(product, cancellationToken);
    }
}