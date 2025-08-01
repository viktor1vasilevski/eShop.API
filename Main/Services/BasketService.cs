﻿using Domain.Interfaces;
using Domain.Models;
using eShop.Domain.Interfaces;
using eShop.Domain.Models;
using eShop.Infrastructure.Data.Context;
using eShop.Main.DTOs.Basket;
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

    public async Task<ApiResponse<BasketDTO>> GetBasketByUserIdAsync(Guid userId)
    {
        var baskets = await _basketRepository.GetAsync(
            filter: b => b.UserId == userId,
            include: b => b.Include(x => x.Items).ThenInclude(i => i.Product)
        );

        var basket = baskets.FirstOrDefault();

        if (basket is null)
        {
            return new ApiResponse<BasketDTO>
            {
                NotificationType = NotificationType.NotFound,
                Message = "No basket found for the user."
            };
        }

        string? BuildImageDataUrl(byte[]? bytes, string? imageType)
        {
            if (bytes == null || bytes.Length == 0 || string.IsNullOrWhiteSpace(imageType))
                return null;

            // Normalize mime type (e.g., "jpeg" -> "image/jpeg")
            var lower = imageType.Trim().ToLowerInvariant();
            string mime = lower.StartsWith("image/") ? lower : $"image/{lower}";

            var base64 = Convert.ToBase64String(bytes);
            return $"data:{mime};base64,{base64}";
        }

        var basketDto = new BasketDTO
        {
            Items = basket.Items.Select(i => new BasketItemDTO
            {
                ProductId = i.ProductId,
                ProductName = i.Product?.Name,
                Quantity = i.Quantity,
                Price = i.Product?.UnitPrice ?? 0,
                UnitQuantity = i.Product?.UnitQuantity ?? 0,
                ImageDataUrl = BuildImageDataUrl(i.Product?.Image, i.Product?.ImageType)
            }).ToList()
        };

        return new ApiResponse<BasketDTO>
        {
            NotificationType = NotificationType.Success,
            Message = "Basket retrieved successfully.",
            Data = basketDto
        };
    }

    public async Task<ApiResponse<string>> Merge(Guid userId, List<BasketRequest> request)
    {
        var userExists = await _userRepository.ExistsAsync(x => x.Id == userId);
        if (!userExists)
            return new ApiResponse<string>
            {
                Message = "User not found",
                NotificationType = NotificationType.NotFound
            };

        var basket = _basketRepository
            .Get(x => x.UserId == userId, include: x => x.Include(b => b.Items).ThenInclude(x => x.Product))
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
                await _basketItemRepository.UpdateAsync(existingItem);
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
            Message = "Basket successfully merged",
            NotificationType = NotificationType.Success
        };
    }

    public async Task<ApiResponse<BasketDTO>> UpdateItemQuantityAsync(Guid userId, Guid productId, int newQuantity)
    {
        // Load the basket with items and products
        var baskets = await _basketRepository.GetAsync(
            filter: b => b.UserId == userId,
            include: b => b.Include(x => x.Items).ThenInclude(i => i.Product)
        );

        var basket = baskets.FirstOrDefault();
        if (basket == null)
        {
            return new ApiResponse<BasketDTO>
            {
                NotificationType = NotificationType.NotFound,
                Message = "Basket not found."
            };
        }

        var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
        {
            return new ApiResponse<BasketDTO>
            {
                NotificationType = NotificationType.NotFound,
                Message = "Item not found in basket."
            };
        }

        // Validate quantity bounds
        int validQuantity = Math.Max(1, Math.Min(newQuantity, item.Product?.UnitQuantity ?? int.MaxValue));

        item.Quantity = validQuantity;

        // Update the basket item directly
        await _basketItemRepository.UpdateAsync(item);

        // Save changes once
        await uow.SaveChangesAsync();

        // Prepare DTO for response
        var basketDto = new BasketDTO
        {
            Items = basket.Items.Select(i => new BasketItemDTO
            {
                ProductId = i.ProductId,
                ProductName = i.Product?.Name,
                Quantity = i.Quantity,
                Price = i.Product?.UnitPrice ?? 0,
                UnitQuantity = i.Product?.UnitQuantity ?? 0,
                ImageDataUrl = BuildImageDataUrl(i.Product?.Image, i.Product?.ImageType)
            }).ToList()
        };

        return new ApiResponse<BasketDTO>
        {
            NotificationType = NotificationType.Success,
            Message = "Item quantity updated.",
            Data = basketDto
        };
    }


    // You can reuse your existing BuildImageDataUrl method or make it private
    private string? BuildImageDataUrl(byte[]? bytes, string? imageType)
    {
        if (bytes == null || bytes.Length == 0 || string.IsNullOrWhiteSpace(imageType))
            return null;

        var lower = imageType.Trim().ToLowerInvariant();
        string mime = lower.StartsWith("image/") ? lower : $"image/{lower}";

        var base64 = Convert.ToBase64String(bytes);
        return $"data:{mime};base64,{base64}";
    }

}
