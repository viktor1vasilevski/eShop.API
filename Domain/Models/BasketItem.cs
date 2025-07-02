using Domain.Models;
using Domain.Models.Base;

namespace eShop.Domain.Models;

public class BasketItem : AuditableBaseEntity
{
    public int BasketId { get; set; }
    public virtual Basket? Basket { get; set; }

    public int ProductId { get; set; }
    public virtual Product? Product { get; set; }

    public int Quantity { get; set; }
}
