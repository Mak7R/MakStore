using EmployeeWebClient.Extensions;
using EmployeeWebClient.Models.Product;
using EmployeeWebClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeWebClient.Controllers;


[Controller]
[Route("products")]
public class ProductsController : Controller
{
    private readonly IProductsService _productsService;

    public ProductsController(IProductsService productsService)
    {
        _productsService = productsService;
    }
    
    [AllowAnonymous]
    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        return View(await _productsService.GetAll());
    }

    [AllowAnonymous]
    [HttpGet("{productId:guid}")]
    public async Task<IActionResult> GetById(Guid productId)
    {
        return View(await _productsService.GetById(productId));
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("create")]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateProductViewModel product)
    {
        if (!ModelState.IsValid)
            return View(product);

        var result = await _productsService.Create(product);
        if (result.IsSuccessful)
            return RedirectToAction("GetById", "Products", new { productId = result.Result });
        
        if (result.ProblemDetails?.Errors != null)
            ModelState.AddErrors(result.ProblemDetails.Errors);
        
        return View(product);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{productId:guid}/update")]
    public async Task<IActionResult> Update(Guid productId)
    {
        var product = await _productsService.GetById(productId);
        
        return View(new UpdateProductViewModel
        {
            Name = product?.Name ?? string.Empty,
            Description = product?.Description,
            Price = product?.Price ?? 0
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{productId:guid}/update")]
    public async Task<IActionResult> Update(Guid productId, UpdateProductViewModel product)
    {
        if (!ModelState.IsValid)
            return View(product);
        
        var result = await _productsService.Update(productId, product);
        if (result.IsSuccessful)
            return RedirectToAction("GetById", "Products", new { productId });
        
        if (result.ProblemDetails?.Errors != null)
            ModelState.AddErrors(result.ProblemDetails.Errors);
        
        return View(product);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{productId:guid}/delete")]
    public async Task<IActionResult> Delete(Guid productId)
    {
        var result = await _productsService.Delete(productId, new DeleteProductViewModel());
        if (result.IsSuccessful)
            return RedirectToAction("GetAll", "Products");
        
        if (result.ProblemDetails?.Errors != null)
            ModelState.AddErrors(result.ProblemDetails.Errors);
        
        return RedirectToAction("GetById", "Products", new { productId });
    }
}