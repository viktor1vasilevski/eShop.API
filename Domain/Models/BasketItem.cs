using Domain.Models;
using Domain.Models.Base;
using eShop.Domain.Exceptions;

namespace eShop.Domain.Models;

public class BasketItem : AuditableBaseEntity
{
    public Guid BasketId { get; set; }
    public virtual Basket? Basket { get; set; }

    public Guid ProductId { get; set; }
    public virtual Product? Product { get; set; }

    public int Quantity { get; set; }



    private BasketItem() { }

    public static BasketItem CreateNew(Guid basketId, Guid productId, int quantity)
    {
        if (productId == Guid.Empty)
            throw new DomainException("ProductId cannot be empty.");
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        return new BasketItem
        {
            Id = Guid.NewGuid(),
            BasketId = basketId,
            ProductId = productId,
            Quantity = quantity
        };
    }

    public void IncreaseQuantity(int amount)
    {
        if (amount <= 0)
            throw new DomainException("Increase amount must be positive.");

        Quantity += amount;
    }
}
