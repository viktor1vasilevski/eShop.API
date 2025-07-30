using eShop.Main.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eShop.PublicApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController(ICategoryService _categoryService) : BaseController
{


    [HttpGet("CategoriesWithSubcategoriesForMenu")]
    public IActionResult GetCategoriesDropdownList()
    {
        var response = _categoryService.GetCategoriesWithSubcategoriesForMenu();
        return HandleResponse(response);
    }
}
