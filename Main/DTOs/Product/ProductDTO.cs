namespace eShop.Main.DTOs.Product;

public class ProductDTO
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? UnitPrice { get; set; }
    public int? UnitQuantity { get; set; }
    public string? Category { get; set; }
    public string? Subcategory { get; set; }
    public Guid SubcategoryId { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? LastModified { get; set; }
}
