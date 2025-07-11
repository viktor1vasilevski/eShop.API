using eShop.Main.Requests.Category;

namespace eShop.Admin.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
public class CategoryController(ICategoryService categoryService) : BaseController
{
    private readonly ICategoryService _categoryService = categoryService;

    [HttpGet]
    public IActionResult Get([FromQuery] CategoryRequest request)
    {
        var response = _categoryService.GetCategories(request);
        return HandleResponse(response);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateCategoryRequest request)
    {
        var response = _categoryService.CreateCategory(request);
        if (response.Success && response.NotificationType == NotificationType.Created && response.Data?.Id != null)
        {
            var locationUri = Url.Action(nameof(GetById), "Category", new { id = response.Data.Id }, Request.Scheme);
            response.Location = locationUri;
        }

        return HandleResponse(response);
    }

    [HttpPut("{id}")]
    public IActionResult Edit([FromRoute] Guid id, [FromBody] EditCategoryRequest request)
    {
        var response = _categoryService.EditCategory(id, request);
        return HandleResponse(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetById([FromRoute] Guid id)
    {
        var response = _categoryService.GetCategoryById(id);
        return HandleResponse(response);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] Guid id)
    {
        var response = _categoryService.DeleteCategory(id);
        return HandleResponse(response);
    }

    [HttpGet("GetCategoriesDropdownList")]
    public IActionResult GetCategoriesDropdownList()
    {
        var response = _categoryService.GetCategoriesDropdownList();
        return HandleResponse(response);
    }
}
