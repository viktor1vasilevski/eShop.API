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
        SetCategoryId(categoryId);
        SetName(name);
    }

    public void Update(Guid categoryId, string name)
    {
        SetCategoryId(categoryId);
        SetName(name);
    }

    private void SetCategoryId(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            throw new DomainValidationException("Category Id cannot be empty.");

        CategoryId = categoryId;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Subcategory name cannot be empty.");

        if (name.Length < 3)
            throw new DomainValidationException("Subcategory name must be at least 3 characters long.");

        Name = name;
    }
}
