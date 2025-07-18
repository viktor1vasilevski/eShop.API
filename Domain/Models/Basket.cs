using Domain.Models;
using Domain.Models.Base;
using eShop.Domain.Exceptions;

namespace eShop.Domain.Models;

public class Basket : AuditableBaseEntity
{
    public Guid UserId { get; private set; }

    public virtual User? User { get; set; }
    public virtual ICollection<BasketItem> Items { get; set; } = new List<BasketItem>();



    private Basket()
    {
            
    }

    public static Basket CreateNew(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new DomainException("UserId cannot be empty.");

        return new Basket
        {
            Id = Guid.NewGuid(),
            UserId = userId
        };
    }

    public void AddOrUpdateItem(Guid productId, int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        var existingItem = Items?.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.IncreaseQuantity(quantity);
        }
        else
        {
            var newItem = BasketItem.CreateNew(this.Id, productId, quantity);
            Items ??= new List<BasketItem>();
            Items.Add(newItem);
        }
    }
}
