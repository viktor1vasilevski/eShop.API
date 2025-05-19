namespace eShop.Main.DTOs.Subcategory;

public class SubcategoryDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public string Category { get; set; } = String.Empty;
    public Guid CategoryId { get; set; }
    public virtual DateTime Created { get; set; }
    public virtual DateTime? LastModified { get; set; }
}
