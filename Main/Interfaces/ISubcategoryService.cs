using eShop.Main.DTOs.Subcategory;
using eShop.Main.Requests.Subcategory;
using Main.Responses;

namespace eShop.Main.Interfaces;

public interface ISubcategoryService
{
    ApiResponse<List<SubcategoryDTO>> GetSubcategories(SubcategoryRequest request);
    ApiResponse<SubcategoryDTO> CreateSubcategory(CreateSubcategoryRequest request);
    ApiResponse<SubcategoryDTO> GetSubcategoryById(Guid id);
}
