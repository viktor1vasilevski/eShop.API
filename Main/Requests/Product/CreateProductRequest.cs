namespace eShop.Main.Requests.Product;

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public Guid SubcategoryId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string Description { get; set; } = string.Empty;
}
