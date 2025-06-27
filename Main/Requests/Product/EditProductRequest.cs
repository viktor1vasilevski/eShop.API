namespace eShop.Main.Requests.Product;

public class EditProductRequest
{
    public string Name { get; set; }
    public Guid SubcategoryId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
}
