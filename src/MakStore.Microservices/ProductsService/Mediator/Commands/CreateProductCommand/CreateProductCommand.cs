using MediatR;

namespace ProductsService.Mediator.Commands.CreateProductCommand;

public sealed class CreateProductCommand : IRequest<Guid>
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ResourcesUris { get; set; }
}