using Domain.Models;
using Domain.Models.Base;

namespace eShop.Domain.Models;

public class Basket : AuditableBaseEntity
{
    public Guid UserId { get; set; }
    public virtual User? User { get; set; }

    public ICollection<BasketItem> Items { get; set; } = new List<BasketItem>();
}
