namespace eShop.Main.DTOs.Product;

public class ProductDTO
{
    public Guid Id { get; set; }
    public string? Brand { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? LastModified { get; set; }
}
