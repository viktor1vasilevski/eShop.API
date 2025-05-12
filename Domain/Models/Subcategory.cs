using Domain.Models.Base;
#nullable enable

namespace Domain.Models;

public class Subcategory : AuditableBaseEntity
{
    public string Name { get; set; }
    public Guid CategoryId { get; set; }

    public virtual Category? Category { get; set; }
    public virtual ICollection<Product>? Products { get; set; }
}
