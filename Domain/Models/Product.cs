using Domain.Models.Base;
using eShop.Domain.Exceptions;

namespace Domain.Models;

public class Product : AuditableBaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int UnitQuantity { get; set; }


    public Guid SubcategoryId { get; set; }
    public virtual Subcategory? Subcategory { get; set; }

    protected Product() { }
    public Product(string name, string description, decimal unitPrice, int unitQuantity, Guid subcategoryId)
    {
        Initialize(name, description, unitPrice, unitQuantity, subcategoryId);
    }

    public void Update(string name, string description, decimal unitPrice, int unitQuantity, Guid subcategoryId) => ApplyChanges(name, description, unitPrice, unitQuantity, subcategoryId);

    private void ApplyChanges(string name, string description, decimal unitPrice, int unitQuantity, Guid subcategoryId)
    {
        Validate(name, description, unitPrice, unitQuantity, subcategoryId);
        Name = name;
        Description = description;
        UnitPrice = unitPrice;
        UnitQuantity = unitQuantity;
        SubcategoryId = subcategoryId;
    }
    private void Initialize(string name, string description, decimal unitPrice, int unitQuantity, Guid subcategoryId)
    {
        Validate(name, description, unitPrice, unitQuantity, subcategoryId);
        Name = name;
        Description = description;
        UnitPrice = unitPrice;
        UnitQuantity = unitQuantity;
        SubcategoryId = subcategoryId;
    }

    private void Validate(string name, string description, decimal unitPrice, int unitQuantity, Guid subcategoryId)
    {
        if (subcategoryId == Guid.Empty)
            throw new DomainValidationException("Subcategory Id cannot be empty.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Name cannot be empty.");

        if (name.Length > 50)
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
