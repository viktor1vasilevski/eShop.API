using Admin.eShop.Controllers;
using eShop.Main.Interfaces;
using eShop.Main.Requests.Category;
using eShop.Main.Requests.Product;
using eShop.Main.Services;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Admin.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController(IProductService productService) : BaseController
{
    private readonly IProductService _productService = productService;

    [HttpGet]
    public IActionResult Get([FromQuery] ProductRequest request)
    {
        var response = _productService.GetProducts(request);
        return HandleResponse(response);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateProductRequest request)
    {
        var response = _productService.CreateProduct(request);
        return HandleResponse(response);
    }
}
