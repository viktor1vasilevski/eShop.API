﻿using eShop.Main.Requests.Product;

namespace eShop.Admin.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
public class ProductController(IProductService _productService) : BaseController
{

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

    [HttpPost]
    public IActionResult Create([FromBody] CreateProductRequest request)
    {
        var response = _productService.CreateProduct(request);
        if (response.NotificationType == NotificationType.Created && response.Data?.Id != null)
        {
            var locationUri = Url.Action(nameof(GetById), "Product", new { id = response.Data.Id }, Request.Scheme);
            response.Location = locationUri;
        }
        return HandleResponse(response);
    }

    [HttpPut("{id}")]
    public IActionResult Edit([FromRoute] Guid id, [FromBody] EditProductRequest request)
    {
        var response = _productService.EditProduct(id, request);
        return HandleResponse(response);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] Guid id)
    {
        var response = _productService.DeleteProduct(id);
        return HandleResponse(response);
    }
}
