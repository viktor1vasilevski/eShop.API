namespace eShop.Main.Requests.Subcategory;

public class CreateSubcategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
}
