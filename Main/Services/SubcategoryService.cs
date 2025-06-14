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
    private readonly IGenericRepository<Category> _categoryRepository = _uow.GetGenericRepository<Category>();

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
    public ApiResponse<SubcategoryDTO> CreateSubcategory(CreateSubcategoryRequest request)
    {
        try
        {
            if (_subcategoryRepository.Exists(x => x.Name.ToLower() == request.Name.ToLower() && x.CategoryId == request.CategoryId))
                return new ApiResponse<SubcategoryDTO>()
                {
                    Success = false,
                    Message = SubcategoryConstants.SUBCATEGORY_EXISTS,
                    NotificationType = NotificationType.BadRequest
                };

            if (!_categoryRepository.Exists(x => x.Id == request.CategoryId))
                return new ApiResponse<SubcategoryDTO>()
                {
                    Success = false,
                    Message = CategoryConstants.CATEGORY_DOESNT_EXIST,
                    NotificationType = NotificationType.BadRequest
                };

            var entity = new Subcategory(request.CategoryId, request.Name);

            _subcategoryRepository.Insert(entity);
            _uow.SaveChanges();

            return new ApiResponse<SubcategoryDTO>
            {
                Success = true,
                NotificationType = NotificationType.Success,
                Message = SubcategoryConstants.SUBCATEGORY_SUCCESSFULLY_CREATED
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred in {FunctionName} at {Timestamp} : Name: {Name}, CategoryId: {CategoryId}",
                nameof(CreateSubcategory) ,DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), request.Name, request.CategoryId);

            return new ApiResponse<SubcategoryDTO>
            {
                Success = false,
                NotificationType = NotificationType.ServerError,
                Message = SubcategoryConstants.ERROR_CREATING_SUBCATEGORY,
            };
        }
    }

    public ApiResponse<SubcategoryDTO> GetSubcategoryById(Guid id)
    {
        try
        {
            if (_subcategoryRepository.Exists(x => x.Id == id))
            {
                var subcategory = _subcategoryRepository.GetAsQueryable(x => x.Id == id, null,
                    x => x.Include(x => x.Products).Include(x => x.Category)).FirstOrDefault();

                return new ApiResponse<SubcategoryDTO>
                {
                    Success = true,
                    NotificationType = NotificationType.Success,
                    Data = new SubcategoryDTO()
                    {
                        Id = subcategory.Id,
                        Name = subcategory.Name,
                        CategoryId = subcategory.Category.Id,
                        Category = subcategory.Category.Name
                    }
                };
            }

            return new ApiResponse<SubcategoryDTO>
            {
                Success = false,
                NotificationType = NotificationType.BadRequest,
                Message = SubcategoryConstants.SUBCATEGORY_DOESNT_EXIST,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred in {FunctionName} at {Timestamp} : SubcategoryId: {CategoryId}",
                nameof(GetSubcategoryById), DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), id);

            return new ApiResponse<SubcategoryDTO>
            {
                Success = false,
                NotificationType = NotificationType.ServerError,
                Message = SubcategoryConstants.ERROR_GET_SUBCATEGORY_BY_ID,
            };
        }
    }

    public bool DeleteSubcategory(Guid id)
    {
        try
        {
            var subcategory = _subcategoryRepository.GetAsQueryable(x => x.Id == id && x.Name != "UNCATEGORIZED", null,
                    x => x.Include(x => x.Products)).FirstOrDefault();

            if (subcategory is null) return false;

            if (HasRelatedEntities(subcategory)) return false;

            _subcategoryRepository.Delete(subcategory);
            _uow.SaveChanges();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred in {FunctionName} at {Timestamp} : CategoryId: {CategoryId}",
                        nameof(DeleteSubcategory), DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), id);

            return false;
        }
    }
    private bool HasRelatedEntities(Subcategory subcategory)
    {
        return subcategory.Products?.Any() == true;
    }
}
