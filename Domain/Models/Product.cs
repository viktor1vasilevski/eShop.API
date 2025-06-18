using Domain.Models.Base;
using eShop.Domain.Exceptions;

namespace Domain.Models;

public class Product : AuditableBaseEntity
{
    public string? Brand { get; set; }
    public string? Description { get; set; }
    public decimal? UnitPrice { get; set; }
    public int? UnitQuantity { get; set; }


    public Guid SubcategoryId { get; set; }
    public virtual Subcategory? Subcategory { get; set; }

    protected Product() { }
    public Product(string brand, string description, decimal unitPrice, int unitQuantity, Guid subcategoryId)
    {
        Initialize(brand, description, unitPrice, unitQuantity, subcategoryId);
    }

    public void Update(string brand, string description, decimal unitPrice, int unitQuantity, Guid subcategoryId) => ApplyChanges(brand, description, unitPrice, unitQuantity, subcategoryId);

    private void ApplyChanges(string brand, string description, decimal unitPrice, int unitQuantity, Guid subcategoryId)
    {
        Validate(brand, description, unitPrice, unitQuantity, subcategoryId);
        Brand = brand;
        Description = description;
        UnitPrice = unitPrice;
        UnitQuantity = unitQuantity;
        SubcategoryId = subcategoryId;
    }
    private void Initialize(string brand, string description, decimal unitPrice, int unitQuantity, Guid subcategoryId)
    {
        Validate(brand, description, unitPrice, unitQuantity, subcategoryId);
        Brand = brand;
        Description = description;
        UnitPrice = unitPrice;
        UnitQuantity = unitQuantity;
        SubcategoryId = subcategoryId;
    }

    private void Validate(string brand, string description, decimal unitPrice, int unitQuantity, Guid subcategoryId)
    {
        if (subcategoryId == Guid.Empty)
            throw new DomainValidationException("Subcategory Id cannot be empty.");

        if (string.IsNullOrWhiteSpace(brand))
            throw new DomainValidationException("Brand cannot be empty.");

        if (brand.Length > 50)
            throw new DomainValidationException("Brand name cannot exceed 50 characters.");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainValidationException("Brand cannot be empty.");

        if (description.Length > 500)
            throw new DomainValidationException("Description cannot exceed 500 characters.");

        if (unitPrice <= 0)
            throw new DomainValidationException("Unit price must be greater than zero.");

        if (unitQuantity <= 0)
            throw new DomainValidationException("Unit quantity must be greater than zero.");
    }
}
