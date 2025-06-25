using Domain.Models.Base;
using eShop.Domain.Exceptions;

namespace Domain.Models;

public class Category : AuditableBaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public virtual ICollection<Subcategory>? Subcategories { get; set; }

    protected Category() { }
    public Category(string name)
    {
        Rename(name);
    }

    public void Update(string name) => Rename(name);

    private void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Category name cannot be empty.");

        if (name.Length < 3)
            throw new DomainValidationException("Category name must be at least 3 characters long.");

        Name = name;
    }
}
