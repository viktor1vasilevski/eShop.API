namespace eShop.Main.Requests.Subcategory;
#nullable disable

public class CreateSubcategoryRequest
{
    public string Name { get; set; }
    public Guid CategoryId { get; set; }
}
