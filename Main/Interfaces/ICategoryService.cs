using eShop.Main.DTOs;
using eShop.Main.Requests.Category;
using Main.Responses;

namespace eShop.Main.Interfaces;

public interface ICategoryService
{
    ApiResponse<List<CategoryDTO>> GetCategories(CategoryRequest request);
    ApiResponse<CategoryDTO> CreateCategory(CreateCategoryRequest request);
}
