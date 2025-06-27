using eShop.Main.DTOs.Subcategory;

namespace eShop.Main.DTOs.Category;

public class CategoryWithSubcategoriesDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<SelectSubcategoryListItemDTO> Subcategories { get; set; }
}
