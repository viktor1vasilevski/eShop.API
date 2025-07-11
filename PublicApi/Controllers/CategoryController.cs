using eShop.Main.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eShop.PublicApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController(ICategoryService categoryService) : BaseController
{
    private readonly ICategoryService _categoryService = categoryService;


    [HttpGet("CategoriesWithSubcategoriesForMenu")]
    public IActionResult GetCategoriesDropdownList()
    {
        var response = _categoryService.GetCategoriesWithSubcategoriesForMenu();
        return HandleResponse(response);
    }
}
