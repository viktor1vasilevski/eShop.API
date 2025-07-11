using eShop.Main.Interfaces;
using eShop.Main.Requests.Product;
using Microsoft.AspNetCore.Mvc;

namespace eShop.PublicApi.Controllers;

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

    [HttpGet("{id}")]
    public IActionResult GetById([FromRoute] Guid id)
    {
        var response = _productService.GetProductById(id);
        return HandleResponse(response);
    }
}
