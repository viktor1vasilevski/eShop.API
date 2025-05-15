namespace eShop.Main.Requests.Subcategory;

public class SubcategoryRequest : BaseRequest
{
    public Guid? CategoryId { get; set; }
    public string? Name { get; set; }
}
