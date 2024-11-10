using Asp.Versioning;
using MakStore.SharedComponents.Api;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrdersService.Mediator.Commands.CreateOrderCommand;

namespace OrdersService.Controllers;

[ApiVersion("1.0")]
public class OrdersController : ApiController
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("orders")]
    public IActionResult GetAll()
    {
        throw new NotImplementedException();
    }

    [HttpGet("orders/{orderId:guid}")]
    public IActionResult GetById(Guid orderId)
    {
        throw new NotImplementedException(); 
    }

    [HttpPost("orders")]
    public async Task<IActionResult> Create(CreateOrderCommand command)
    {
        var orderId = await _mediator.Send(command);
        return CreatedAtAction("GetById", "Orders", new { orderId }, orderId);
    }

    [HttpDelete("orders/{orderId:guid}/cancel")]
    public IActionResult Cancel()
    {
        throw new NotImplementedException(); 
    }
}