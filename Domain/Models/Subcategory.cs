using Domain.Models.Base;
using eShop.Domain.Exceptions;

namespace Domain.Models;

public class Subcategory : AuditableBaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public Guid CategoryId { get; private set; }

    public virtual Category? Category { get; set; }
    public virtual ICollection<Product>? Products { get; set; }


    public Subcategory(Guid categoryId, string name)
    {
        Initialize(categoryId, name);
    }

    public void Update(Guid categoryId, string name) => ApplyChanges(categoryId, name);

    private void Initialize(Guid categoryId, string name)
    {
        Validate(categoryId, name);
        CategoryId = categoryId;
        Name = name;
    }

    private void ApplyChanges(Guid categoryId, string name)
    {
        Validate(categoryId, name);
        CategoryId = categoryId;
        Name = name;
    }

    private void Validate(Guid categoryId, string name)
    {
        if (categoryId == Guid.Empty)
            throw new DomainValidationException("Subcategory Id cannot be empty.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Subcategory name cannot be empty.");

        if (name.Length < 3)
            throw new DomainValidationException("Subcategory name must be at least 3 characters long.");
    }
}
