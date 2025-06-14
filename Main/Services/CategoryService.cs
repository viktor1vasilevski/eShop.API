using Domain.Interfaces;
using Domain.Models;
using eShop.Domain.Exceptions;
using eShop.Main.Constants;
using eShop.Main.DTOs.Category;
using eShop.Main.Interfaces;
using eShop.Main.Requests.Category;
using Infrastructure.Data.Context;
using Main.Enums;
using Main.Extensions;
using Main.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eShop.Main.Services;

public class CategoryService(IUnitOfWork<AppDbContext> _uow, ILogger<CategoryService> _logger) : ICategoryService
{
    private readonly IGenericRepository<Category> _categoryRepository = _uow.GetGenericRepository<Category>();

    public ApiResponse<List<CategoryDTO>> GetCategories(CategoryRequest request)
    {
        try
        {
            var categories = _categoryRepository.GetAsQueryableWhereIf(
                filter: x => x.WhereIf(!String.IsNullOrEmpty(request.Name), x => x.Name.ToLower().Contains(request.Name.ToLower())));

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
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), request.Name);

            return new ApiResponse<List<CategoryDTO>>
            {
                Success = false,
                NotificationType = NotificationType.ServerError,
                Message = CategoryConstants.ERROR_RETRIEVING_CATEGORIES
            };
        }
    }

    public ApiResponse<string> CreateCategory(CreateCategoryRequest request)
    {
        try
        {
            if (_categoryRepository.Exists(x => x.Name.ToLower() == request.Name.ToLower()))
                return new ApiResponse<string>()
                {
                    Success = false,
                    Message = CategoryConstants.CATEGORY_EXISTS,
                    NotificationType = NotificationType.Conflict
                };

            var category = new Category(request.Name);

            _categoryRepository.Insert(category);
            _uow.SaveChanges();

            return new ApiResponse<string>
            {
                Success = true,
                NotificationType = NotificationType.Created,
                Message = CategoryConstants.CATEGORY_SUCCESSFULLY_CREATED
            };
        }
        catch (DomainValidationException ex)
        {
            return new ApiResponse<string>
            {
                Success = false,
                NotificationType = NotificationType.BadRequest,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred in {FunctionName} at {Timestamp} CategoryName: {CategoryName}", nameof(CreateCategory),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), request.Name);

            return new ApiResponse<string>
            {
                Success = false,
                NotificationType = NotificationType.ServerError,
                Message = CategoryConstants.ERROR_CREATING_CATEGORY
            };
        }
    }

    public ApiResponse<CategoryDTO> EditCategory(Guid id, EditCategoryRequest request)
    {
        try
        {
            var category = _categoryRepository.GetById(id);
            if (category is null)
                return new ApiResponse<CategoryDTO> 
                { 
                    Success = false, 
                    NotificationType = NotificationType.NotFound, 
                    Message = CategoryConstants.CATEGORY_DOESNT_EXIST 
                };

            if (_categoryRepository.Exists(x => x.Name.ToLower() == request.Name.ToLower() && x.Id != id))
                return new ApiResponse<CategoryDTO> 
                { 
                    Success = false, 
                    NotificationType = NotificationType.Conflict, 
                    Message = CategoryConstants.CATEGORY_EXISTS 
                };

            category.Update(request.Name);

            _categoryRepository.Update(category);
            _uow.SaveChanges();

            return new ApiResponse<CategoryDTO>
            {
                Success = true,
                NotificationType = NotificationType.Success,
                Message = CategoryConstants.CATEGORY_SUCCESSFULLY_UPDATE,
                Data = new CategoryDTO { Id = id, Name = request.Name }
            };
        }
        catch (DomainValidationException ex)
        {
            return new ApiResponse<CategoryDTO>
            {
                Success = false,
                NotificationType = NotificationType.BadRequest,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred in {FunctionName} at {Timestamp} : CategoryId: {CategoryId}, CategoryName: {CategoryName}", nameof(EditCategory),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), id, request.Name);

            return new ApiResponse<CategoryDTO>
            {
                Success = false,
                NotificationType = NotificationType.ServerError,
                Message = CategoryConstants.ERROR_EDITING_CATEGORY
            };
        }
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
                    NotificationType = NotificationType.NotFound,
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
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), id);

            return new ApiResponse<CategoryDetailsDTO>
            {
                Success = false,
                NotificationType = NotificationType.ServerError,
                Message = CategoryConstants.ERROR_GET_CATEGORY
            };
        }
    }

    public ApiResponse<List<SelectCategoryListItemDTO>> GetCategoriesDropdownList()
    {
        try
        {
            var categories = _categoryRepository.GetAsQueryable();

            var categoriesDropdownDTO = categories.Select(x => new SelectCategoryListItemDTO
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            return new ApiResponse<List<SelectCategoryListItemDTO>>
            {
                Success = true,
                Data = categoriesDropdownDTO
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred in {FunctionName} at {Timestamp}", nameof(GetCategoriesDropdownList),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            return new ApiResponse<List<SelectCategoryListItemDTO>>
            {
                Success = false,
                Message = CategoryConstants.ERROR_RETRIEVING_CATEGORIES,
                NotificationType = NotificationType.ServerError
            };
        }
    }

    public ApiResponse<string> DeleteCategory(Guid id)
    {
        try
        {
            var category = _categoryRepository.GetAsQueryable(
                 filter : x => x.Id == id && x.Name != "UNCATEGORIZED",
                 include: x => x.Include(x => x.Subcategories).ThenInclude(x => x.Products)
                 ).FirstOrDefault();

            if (category is null)
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = CategoryConstants.CATEGORY_DOESNT_EXIST,
                    NotificationType = NotificationType.NotFound
                };

            if (HasRelatedEntities(category))
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = CategoryConstants.CATEGORY_HAS_RELATED_ENTITIES,
                    NotificationType = NotificationType.Conflict
                };

            _categoryRepository.Delete(category);
            _uow.SaveChanges();

            return new ApiResponse<string>
            {
                Success = true,
                Message = CategoryConstants.CATEGORY_SUCCESSFULLY_DELETED,
                NotificationType = NotificationType.Success
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred in {FunctionName} at {Timestamp} : CategoryId: {CategoryId}",
                        nameof(DeleteCategory), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), id);

            return new ApiResponse<string>
            {
                Success = false,
                Message = CategoryConstants.ERROR_DELETING_CATEGORY,
                NotificationType = NotificationType.ServerError
            };
        }
    }

    private bool HasRelatedEntities(Category category)
    {
        return category.Subcategories?.Any() == true ||
               category.Subcategories?.FirstOrDefault()?.Products?.Any() == true;
    }

    
}
