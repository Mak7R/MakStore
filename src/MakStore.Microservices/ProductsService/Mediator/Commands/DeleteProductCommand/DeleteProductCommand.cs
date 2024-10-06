using MediatR;

namespace ProductsService.Mediator.Commands.DeleteProductCommand;

public record DeleteProductCommand(Guid Id) : IRequest;