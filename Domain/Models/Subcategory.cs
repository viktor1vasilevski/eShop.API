using Domain.Models.Base;
using eShop.Domain.Exceptions;
#nullable enable

namespace Domain.Models;

public class Subcategory : AuditableBaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public Guid CategoryId { get; private set; }

    public virtual Category? Category { get; set; }
    public virtual ICollection<Product>? Products { get; set; }


    public Subcategory(Guid id, string name)
    {
        Initialize(id, name);
    }

    public void Update(Guid id, string name) => ApplyChanges(id, name);

    private void Initialize(Guid id, string name)
    {
        Validate(id, name);
        Id = id;
        Name = name;
    }

    private void ApplyChanges(Guid id, string name)
    {
        Validate(id, name);
        Id = id;
        Name = name;
    }

    private void Validate(Guid id, string name)
    {
        if (id == Guid.Empty)
            throw new DomainValidationException("Subcategory Id cannot be empty.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Subcategory name cannot be empty.");

        if (name.Length < 3)
            throw new DomainValidationException("Subcategory name must be at least 3 characters long.");
    }
}
