using Domain.Interfaces;
using Domain.Models;
using eShop.Main.Constants;
using eShop.Main.DTOs.Subcategory;
using eShop.Main.Interfaces;
using eShop.Main.Requests.Subcategory;
using Infrastructure.Data.Context;
using Main.Enums;
using Main.Extensions;
using Main.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eShop.Main.Services;

public class SubcategoryService(IUnitOfWork<AppDbContext> _uow, ILogger<SubcategoryService> _logger) : ISubcategoryService
{
    private readonly IGenericRepository<Subcategory> _subcategoryRepository = _uow.GetGenericRepository<Subcategory>();
    public ApiResponse<List<SubcategoryDTO>> GetSubcategories(SubcategoryRequest request)
    {
        try
        {
            var subcategories = _subcategoryRepository.GetAsQueryableWhereIf(x =>
                x.WhereIf(!String.IsNullOrEmpty(request.CategoryId.ToString()), x => x.CategoryId == request.CategoryId)
                 .WhereIf(!String.IsNullOrEmpty(request.Name), x => x.Name.ToLower().Contains(request.Name.ToLower())),
                null,
                x => x.Include(x => x.Category));

            if (!string.IsNullOrEmpty(request.SortBy) && !string.IsNullOrEmpty(request.SortDirection))
            {
                if (request.SortDirection.ToLower() == "asc")
                {
                    subcategories = request.SortBy.ToLower() switch
                    {
                        "created" => subcategories.OrderBy(x => x.Created),
                        "lastmodified" => subcategories.OrderBy(x => x.LastModified),
                        _ => subcategories.OrderBy(x => x.Created)
                    };
                }
                else if (request.SortDirection.ToLower() == "desc")
                {
                    subcategories = request.SortBy.ToLower() switch
                    {
                        "created" => subcategories.OrderByDescending(x => x.Created),
                        "lastmodified" => subcategories.OrderByDescending(x => x.LastModified),
                        _ => subcategories.OrderByDescending(x => x.Created)
                    };
                }
            }

            var totalCount = subcategories.Count();

            if (request.Skip.HasValue)
                subcategories = subcategories.Skip(request.Skip.Value);

            if (request.Take.HasValue)
                subcategories = subcategories.Take(request.Take.Value);

            var subcategoriesDTO = subcategories.Select(x => new SubcategoryDTO
            {
                Id = x.Id,
                Name = x.Name,
                Category = x.Category.Name,
                CategoryId = x.Category.Id,
                Created = x.Created,
                LastModified = x.LastModified,
            }).ToList();

            return new ApiResponse<List<SubcategoryDTO>>()
            {
                Success = true,
                Data = subcategoriesDTO,
                NotificationType = NotificationType.Success,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred in {FunctionName} at {Timestamp} : Name: {Name}, CategoryId: {CategoryId}",
                nameof(GetSubcategories) ,DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), request.Name, request.CategoryId);

            return new ApiResponse<List<SubcategoryDTO>>()
            {
                Success = false,
                Message = SubcategoryConstants.ERROR_RETRIEVING_SUBCATEGORIES,
                NotificationType = NotificationType.ServerError,
            };
        }
    }
}
