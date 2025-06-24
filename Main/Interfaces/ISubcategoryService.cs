using eShop.Main.DTOs.Category;
using eShop.Main.DTOs.Subcategory;
using eShop.Main.Requests.Subcategory;
using eShop.Main.Responses;

namespace eShop.Main.Interfaces;

public interface ISubcategoryService
{
    ApiResponse<List<SubcategoryDTO>> GetSubcategories(SubcategoryRequest request);
    ApiResponse<SubcategoryDTO> CreateSubcategory(CreateSubcategoryRequest request);
    ApiResponse<CategoryDTO> EditSubcategory(Guid id, EditSubcategoryRequest request);
    ApiResponse<SubcategoryDTO> GetSubcategoryById(Guid id);
    ApiResponse<string> DeleteSubcategory(Guid id);
    ApiResponse<List<SelectSubcategoryListItemDTO>> GetSubcategoriesWithCategoriesDropdownList();
    ApiResponse<List<SelectSubcategoryListItemDTO>> GetSubcategoriesDropdownList();
}
