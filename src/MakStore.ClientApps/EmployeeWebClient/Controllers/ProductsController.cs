using EmployeeWebClient.Extensions;
using EmployeeWebClient.Models.Product;
using EmployeeWebClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeWebClient.Controllers;

[Authorize]
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

    [HttpGet("{productId:guid}")]
    public async Task<IActionResult> GetById(Guid productId)
    {
        return View(await _productsService.GetById(productId));
    }
    
    [HttpGet("create")]
    public IActionResult Create()
    {
        return View();
    }

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

    [HttpGet("{productId:guid}/update")]
    public async Task<IActionResult> Update(Guid productId)
    {
        var product = await _productsService.GetById(productId);
        
        return View(new UpdateProductViewModel
        {
            Name = product.Name,
            Description = product.Description,
            Price = product.Price
        });
    }

    
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