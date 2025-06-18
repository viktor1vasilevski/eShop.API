using eShop.Main.DTOs.Category;
using eShop.Main.Requests.Category;
using eShop.Main.Responses;

namespace eShop.Main.Interfaces;

public interface ICategoryService
{
    ApiResponse<List<CategoryDTO>> GetCategories(CategoryRequest request);
    ApiResponse<string> CreateCategory(CreateCategoryRequest request);
    ApiResponse<CategoryDTO> EditCategory(Guid id, EditCategoryRequest request);
    ApiResponse<CategoryDetailsDTO> GetCategoryById(Guid id);
    ApiResponse<string> DeleteCategory(Guid id);
    ApiResponse<List<SelectCategoryListItemDTO>> GetCategoriesDropdownList();
}
