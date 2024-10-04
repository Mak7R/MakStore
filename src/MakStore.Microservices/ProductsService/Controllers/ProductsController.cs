using Asp.Versioning;
using MakStore.SharedComponents.Api;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductsService.Mediator.Commands.CreateProductCommand;
using ProductsService.Mediator.Commands.DeleteProductCommand;
using ProductsService.Mediator.Commands.UpdateProductCommand;
using ProductsService.Mediator.Queries.GetProductByIdQuery;

namespace ProductsService.Controllers;

[ApiVersion("1.0")]
public class ProductsController : ApiController
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("products/{productId:guid}")]
    public async Task<IActionResult> GetById(Guid productId)
    {
        var query = new GetProductByIdQuery(productId);
        var product = await _mediator.Send(query);
        return Ok(product);
    }
    
    [HttpPost("products/create")]
    public async Task<IActionResult> CreateProduct(CreateProductCommand command)
    {
        var productId = await _mediator.Send(command);
        return CreatedAtAction("GetById", "Products", new { productId }, productId);
    }

    [HttpPut("products/update")]
    public IActionResult UpdateProduct(UpdateProductCommand command)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("products/delete")]
    public IActionResult DeleteProduct(DeleteProductCommand command)
    {
        throw new NotImplementedException();
    }
}