using Domain.Interfaces;
using Domain.Models;
using eShop.Main.Constants;
using eShop.Main.DTOs.Category;
using eShop.Main.Interfaces;
using eShop.Main.Requests.Category;
using Infrastructure.Data.Context;
using Main.Enums;
using Main.Extensions;
using Main.Responses;
using Microsoft.Extensions.Logging;

namespace eShop.Main.Services;

public class CategoryService(IUnitOfWork<AppDbContext> _uow, ILogger<CategoryService> _logger) : ICategoryService
{
    private readonly IGenericRepository<Category> _categoryRepository = _uow.GetGenericRepository<Category>();

    public ApiResponse<List<CategoryDTO>> GetCategories(CategoryRequest request)
    {
        try
        {
            var categories = _categoryRepository.GetAsQueryableWhereIf(c => c
                .WhereIf(!String.IsNullOrEmpty(request.Name), x => x.Name.ToLower().Contains(request.Name.ToLower())), null, null);

            if (!string.IsNullOrEmpty(request.SortBy) && !string.IsNullOrEmpty(request.SortDirection))
            {
                if (request.SortDirection.ToLower() == "asc")
                {
                    categories = request.SortBy.ToLower() switch
                    {
                        "created" => categories.OrderBy(x => x.Created),
                        "lastmodified" => categories.OrderBy(x => x.LastModified),
                        _ => categories.OrderBy(x => x.Created)
                    };
                }
                else if (request.SortDirection.ToLower() == "desc")
                {
                    categories = request.SortBy.ToLower() switch
                    {
                        "created" => categories.OrderByDescending(x => x.Created),
                        "lastmodified" => categories.OrderByDescending(x => x.LastModified),
                        _ => categories.OrderByDescending(x => x.Created)
                    };
                }
            }

            var totalCount = categories.Count();

            if (request.Skip.HasValue)
                categories = categories.Skip(request.Skip.Value);

            if (request.Take.HasValue)
                categories = categories.Take(request.Take.Value);

            var categoriesDTO = categories.Select(x => new CategoryDTO
            {
                Id = x.Id,
                Name = x.Name,
                Created = x.Created,
                LastModified = x.LastModified,
            }).ToList();

            return new ApiResponse<List<CategoryDTO>>()
            {
                Success = true,
                Data = categoriesDTO,
                TotalCount = totalCount,
                NotificationType = NotificationType.Success,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred in {FunctionName} at {Timestamp} : Name: {Name}", nameof(GetCategories),
                DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), request.Name);

            return new ApiResponse<List<CategoryDTO>>
            {
                Success = false,
                NotificationType = NotificationType.ServerError,
                Message = CategoryConstants.ERROR_RETRIEVING_CATEGORIES
            };
        }
    }
    public ApiResponse<CategoryDTO> CreateCategory(CreateCategoryRequest request)
    {
        try
        {
            if (_categoryRepository.Exists(x => x.Name.ToLower() == request.Name.ToLower()))
                return new ApiResponse<CategoryDTO>()
                {
                    Success = false,
                    Message = CategoryConstants.CATEGORY_EXISTS,
                    NotificationType = NotificationType.BadRequest
                };

            _categoryRepository.Insert(new Category { Name = request.Name });
            _uow.SaveChanges();

            return new ApiResponse<CategoryDTO>
            {
                Success = true,
                NotificationType = NotificationType.Created,
                Message = CategoryConstants.CATEGORY_SUCCESSFULLY_CREATED
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred in {FunctionName} at {Timestamp} Name: {Name}", nameof(CreateCategory),
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), request.Name);

            return new ApiResponse<CategoryDTO>
            {
                Success = false,
                NotificationType = NotificationType.ServerError,
                Message = CategoryConstants.ERROR_CREATING_CATEGORY
            };
        }
    }

    public ApiResponse<CategoryDTO> EditCategory(Guid id, EditCategoryRequest request)
    {
        throw new NotImplementedException();
    }

    public ApiResponse<CategoryDetailsDTO> GetCategoryById(Guid id)
    {
        try
        {
            var category = _categoryRepository.GetById(id);

            if (category is null)
                return new ApiResponse<CategoryDetailsDTO>
                {
                    Success = false,
                    NotificationType = NotificationType.BadRequest,
                    Message = CategoryConstants.CATEGORY_DOESNT_EXIST
                };

            return new ApiResponse<CategoryDetailsDTO>
            {
                Success = true,
                NotificationType = NotificationType.Success,
                Data = new CategoryDetailsDTO { Id = category.Id, Name = category.Name }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred in {FunctionName} at {Timestamp} : CategoryId: {CategoryId}", nameof(GetCategoryById),
                        DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), id);

            return new ApiResponse<CategoryDetailsDTO>
            {
                Success = false,
                NotificationType = NotificationType.ServerError,
                Message = CategoryConstants.ERROR_GET_CATEGORY
            };
        }
    }
}
