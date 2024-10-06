using FluentValidation;
using MakStore.SharedComponents.Exceptions;
using MediatR;
using ProductsService.Infrastructure.Repositories;
using ProductsService.Mapping;
using ProductsService.Models;
using ValidationException = MakStore.SharedComponents.Exceptions.ValidationException;

namespace ProductsService.Mediator.Commands.CreateProductCommand;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand,  Guid>
{
    private readonly IProductsRepository _productsRepository;
    private readonly IValidator<CreateProductCommand> _validator;

    public CreateProductCommandHandler(IProductsRepository productsRepository, IValidator<CreateProductCommand> validator)
    {
        _productsRepository = productsRepository;
        _validator = validator;
    }
    
    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.ToDictionary(), $"{nameof(CreateProductCommand)} validation failed");
        
        if (await _productsRepository.FindByNameAsync(request.Name, cancellationToken) != null)
            throw new AlreadyExistsException($"Product with name '{request.Name}' already exists");
        
        var product = ApplicationMapper.Mapper.Map<Product>(request);
        
        var productResponse = await _productsRepository.CreateAsync(product, cancellationToken);
        return productResponse.Id;
    }
}