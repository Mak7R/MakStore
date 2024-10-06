using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ProductsService.Mediator.Commands.UpdateProductCommand;

public class UpdateProductCommand : IRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
}