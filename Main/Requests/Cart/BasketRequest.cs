namespace eShop.Main.Requests.Cart;

public class BasketRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
