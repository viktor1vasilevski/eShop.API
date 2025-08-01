using eShop.Main.DTOs.Basket;
using eShop.Main.Requests.Cart;
using eShop.Main.Responses;

namespace eShop.Main.Interfaces;

public interface IBasketService
{
    Task<ApiResponse<string>> Merge(Guid userId, List<BasketRequest> request);
    Task<ApiResponse<BasketDTO>> GetBasketByUserIdAsync(Guid userId);
}
