namespace eShop.Main.DTOs.Product;

public class ProductDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; } 
    public int UnitQuantity { get; set; } 
    public string Category { get; set; } = string.Empty;
    public string Subcategory { get; set; } = string.Empty;
    public Guid SubcategoryId { get; set; }
    public DateTime Created { get; set; }
    public DateTime? LastModified { get; set; }
}
