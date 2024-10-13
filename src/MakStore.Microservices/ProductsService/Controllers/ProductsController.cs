using Asp.Versioning;
using MakStore.SharedComponents.Api;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsService.Mediator.Commands.CreateProductCommand;
using ProductsService.Mediator.Commands.DeleteProductCommand;
using ProductsService.Mediator.Commands.UpdateProductCommand;
using ProductsService.Mediator.Queries.GetProductByIdQuery;
using ProductsService.Mediator.Queries.GetProductsQuery;

namespace ProductsService.Controllers;

[ApiVersion("1.0")]
public class ProductsController : ApiController
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpGet("products")]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetProductsQuery();
        var products = await _mediator.Send(query);
        return Ok(products);
    }
    
    [AllowAnonymous]
    [HttpGet("products/{productId:guid}")]
    public async Task<IActionResult> GetById(Guid productId)
    {
        var query = new GetProductByIdQuery(productId);
        var product = await _mediator.Send(query);
        return Ok(product);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost("products")]
    public async Task<IActionResult> CreateProduct(CreateProductCommand command)
    {
        var productId = await _mediator.Send(command);
        return CreatedAtAction("GetById", "Products", new { productId }, productId);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("products/{productId:guid}")]
    public async Task<IActionResult> UpdateProduct(Guid productId, UpdateProductCommand command)
    {
        command.Id = productId;
        await _mediator.Send(command);
        return Ok(command.Id);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("products/{productId:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid productId)
    {
        await _mediator.Send(new DeleteProductCommand(productId));
        return NoContent();
    }
}