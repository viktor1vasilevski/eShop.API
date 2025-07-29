using Domain.Interfaces;
using Domain.Models;
using eShop.Domain.Interfaces;
using eShop.Domain.Models;
using eShop.Infrastructure.Data.Context;
using eShop.Main.Interfaces;
using eShop.Main.Requests.Cart;
using eShop.Main.Responses;
using Main.Enums;
using Microsoft.EntityFrameworkCore;

namespace eShop.Main.Services;

public class BasketService(IUnitOfWork<AppDbContext> uow) : IBasketService
{
    private readonly IGenericRepository<Basket> _basketRepository = uow.GetGenericRepository<Basket>();
    private readonly IGenericRepository<Product> _productRepository = uow.GetGenericRepository<Product>();
    private readonly IGenericRepository<User> _userRepository = uow.GetGenericRepository<User>();
    private readonly IGenericRepository<BasketItem> _basketItemRepository = uow.GetGenericRepository<BasketItem>();
    public async Task<ApiResponse<string>> Merge(Guid userId, List<BasketRequest> request)
    {
        var userExists = await _userRepository.ExistsAsync(x => x.Id == userId);
        if (!userExists)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "User not found",
                NotificationType = NotificationType.NotFound
            };
        }

        var basket = _basketRepository
            .Get(x => x.UserId == userId, include: x => x.Include(b => b.Items))
            .FirstOrDefault();

        if (basket is null)
        {
            basket = Basket.CreateNew(userId);
            await _basketRepository.InsertAsync(basket);
        }

        // Extract product IDs from the request
        var productIds = request.Select(r => r.ProductId).Distinct().ToList();

        // Fetch all products in one go to avoid N+1
        var products = await _productRepository
            .GetAsync(x => productIds.Contains(x.Id));

        // Build dictionary for fast lookup
        var productsDict = products.ToDictionary(p => p.Id, p => p);

        foreach (var req in request)
        {
            if (!productsDict.TryGetValue(req.ProductId, out var product))
            {
                // Product not found - you could log, throw, or skip
                continue;
            }

            var existingItem = basket.Items.FirstOrDefault(i => i.ProductId == req.ProductId);

            int currentQuantity = existingItem?.Quantity ?? 0;
            int requestedQuantity = req.Quantity;
            int totalQuantity = currentQuantity + requestedQuantity;

            // Cap quantity at product.UnitQuantity (stock)
            int finalQuantity = Math.Min(totalQuantity, product.UnitQuantity);

            if (existingItem != null)
            {
                existingItem.Quantity = finalQuantity;
                _basketItemRepository.Update(existingItem);
            }
            else
            {
                var newItem = BasketItem.CreateNew(basket.Id, req.ProductId, finalQuantity);
                await _basketItemRepository.InsertAsync(newItem);
            }
        }

        await uow.SaveChangesAsync();

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Basket successfully merged",
            NotificationType = NotificationType.Success
        };
    }

}
