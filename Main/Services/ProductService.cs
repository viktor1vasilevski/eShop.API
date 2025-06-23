using Domain.Interfaces;
using Domain.Models;
using eShop.Domain.Exceptions;
using eShop.Main.Constants;
using eShop.Main.DTOs.Category;
using eShop.Main.DTOs.Product;
using eShop.Main.Interfaces;
using eShop.Main.Requests.Product;
using eShop.Main.Responses;
using Infrastructure.Data.Context;
using Main.Enums;
using Main.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eShop.Main.Services;

public class ProductService(IUnitOfWork<AppDbContext> _uow, ILogger<CategoryService> _logger) : IProductService
{
    private readonly IGenericRepository<Product> _productRepository = _uow.GetGenericRepository<Product>();
    private readonly IGenericRepository<Subcategory> _subcategoryRepository = _uow.GetGenericRepository<Subcategory>();


    public ApiResponse<List<ProductDTO>> GetProducts(ProductRequest request)
    {
        try
        {
            var products = _productRepository.GetAsQueryableWhereIf(x =>
                x.WhereIf(!String.IsNullOrEmpty(request.CategoryId.ToString()), x => x.Subcategory.Category.Id == request.CategoryId)
                 .WhereIf(!String.IsNullOrEmpty(request.SubcategoryId.ToString()), x => x.Subcategory.Id == request.SubcategoryId)
                 .WhereIf(!String.IsNullOrEmpty(request.Description), x => x.Description.ToLower().Contains(request.Description.ToLower()))
                 .WhereIf(!String.IsNullOrEmpty(request.Brand), x => x.Brand.ToLower().Contains(request.Brand.ToLower())),
                null,
                x => x.Include(x => x.Subcategory).ThenInclude(sc => sc.Category));

            if (!string.IsNullOrEmpty(request.SortBy) && !string.IsNullOrEmpty(request.SortDirection))
            {
                if (request.SortDirection.ToLower() == "asc")
                {
                    products = request.SortBy.ToLower() switch
                    {
                        "created" => products.OrderBy(x => x.Created),
                        "lastmodified" => products.OrderBy(x => x.LastModified),
                        "unitprice" => products.OrderBy(x => x.UnitPrice),
                        "unitquantity" => products.OrderBy(x => x.UnitQuantity),
                        _ => products.OrderBy(x => x.Created)
                    };
                }
                else if (request.SortDirection.ToLower() == "desc")
                {
                    products = request.SortBy.ToLower() switch
                    {
                        "created" => products.OrderByDescending(x => x.Created),
                        "lastmodified" => products.OrderByDescending(x => x.LastModified),
                        "unitprice" => products.OrderByDescending(x => x.UnitPrice),
                        "unitquantity" => products.OrderByDescending(x => x.UnitQuantity),
                        _ => products.OrderByDescending(x => x.Created)
                    };
                }
            }

            var totalCount = products.Count();

            if (request.Skip.HasValue)
                products = products.Skip(request.Skip.Value);

            if (request.Take.HasValue)
                products = products.Take(request.Take.Value);

            var productsDTO = products.Select(x => new ProductDTO
            {
                Id = x.Id,
                Brand = x.Brand,
                Description = x.Description,
                UnitPrice = x.UnitPrice,
                UnitQuantity = x.UnitQuantity,
                //ImageBase64 = x.Image != null ? $"data:{x.ImageType};base64,{Convert.ToBase64String(x.Image)}" : null,
                Category = x.Subcategory.Category.Name,
                Subcategory = x.Subcategory.Name,
                SubcategoryId = x.SubcategoryId,
                Created = x.Created,
                LastModified = x.LastModified,
            }).ToList();

            return new ApiResponse<List<ProductDTO>>()
            {
                Success = true,
                Data = productsDTO,
                TotalCount = totalCount,
                NotificationType = NotificationType.Success
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred in {FunctionName} at {Timestamp}", nameof(GetProducts),
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            return new ApiResponse<List<ProductDTO>>()
            {
                Success = false,
                Message = ProductConstants.ERROR_RETRIEVING_PRODUCTS,
                NotificationType = NotificationType.ServerError
            };
        }

    }
    public ApiResponse<string> CreateProduct(CreateProductRequest request)
    {
        try
        {
            if (!_subcategoryRepository.Exists(x => x.Id == request.SubcategoryId))
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = SubcategoryConstants.SUBCATEGORY_DOESNT_EXIST,
                    NotificationType = NotificationType.NotFound
                };

            var product = new Product(request.Brand, request.Description, request.Price, request.Quantity, request.SubcategoryId);

            _productRepository.Insert(product);
            _uow.SaveChanges();

            return new ApiResponse<string>
            {
                Success = true,
                Message = ProductConstants.PRODUCT_SUCCESSFULLY_CREATED,
                NotificationType = NotificationType.Created
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
            _logger.LogError(ex, "An exception occurred in {FunctionName} at {Timestamp}", nameof(CreateProduct),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            return new ApiResponse<string>
            {
                Success = false,
                NotificationType = NotificationType.ServerError,
                Message = ProductConstants.ERROR_CREATING_PRODUCT
            };
        }
    }
    public ApiResponse<string> DeleteProduct(Guid id)
    {
        try
        {
            var product = _productRepository.GetById(id);
            if (product is null)
                return new ApiResponse<string>
                {
                    Success = false,
                    NotificationType = NotificationType.NotFound,
                    Message = ProductConstants.PRODUCT_DOESNT_EXIST
                };

            _productRepository.Delete(product);
            _uow.SaveChanges();

            return new ApiResponse<string>
            {
                Success = true,
                Message = ProductConstants.PRODUCT_SUCCESSFULLY_DELETED,
                NotificationType = NotificationType.Success
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred in {FunctionName} at {Timestamp} : ProductId: {ProductId}",
                        nameof(DeleteProduct), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), id);

            return new ApiResponse<string>
            {
                Success = false,
                Message = ProductConstants.ERROR_DELETING_PRODUCT,
                NotificationType = NotificationType.ServerError
            };
        }
    }
    public ApiResponse<ProductDTO> GetProductById(Guid id)
    {
        var product = _productRepository.Get(
                filter: x => x.Id == id,
                include: x => x.Include(x => x.Subcategory).ThenInclude(x => x.Category)).FirstOrDefault();

        if (product is null)
            return new ApiResponse<ProductDTO>
            {
                Success = false,
                NotificationType = NotificationType.NotFound,
                Message = ProductConstants.PRODUCT_DOESNT_EXIST
            };

        var productDto = new ProductDTO()
        {
            Id = product.Id,
            Brand = product.Brand,
            Description = product.Description,
            UnitPrice = product.UnitPrice,
            UnitQuantity = product.UnitQuantity,
            SubcategoryId = product.SubcategoryId,
            Subcategory = product.Subcategory?.Name,
            Category = product.Subcategory?.Category?.Name,
            LastModified = product.LastModified,
            Created = product.Created
        };


        return new ApiResponse<ProductDTO>
        {
            Success = true,
            NotificationType = NotificationType.Success,
            Data = productDto
        };
    }

    public ApiResponse<ProductDTO> EditProduct(Guid id, EditProductRequest request)
    {
        try
        {
            var product = _productRepository.GetById(id);
            if (product is null)
                return new ApiResponse<ProductDTO>
                {
                    Success = false,
                    NotificationType = NotificationType.NotFound,
                    Message = ProductConstants.PRODUCT_DOESNT_EXIST
                };

            if (!_subcategoryRepository.Exists(x => x.Id == request.SubcategoryId))
                return new ApiResponse<ProductDTO>
                {
                    Success = false,
                    NotificationType = NotificationType.NotFound,
                    Message = SubcategoryConstants.SUBCATEGORY_DOESNT_EXIST,
                };

            // HERE WE WOULD HAVE A CHECK IF THE SAME PRODUCT WITH
            // THE SAME NAME ALREADY EXIST, BUT FOR NOW I DONT HAVE 
            // PRODUCT NAME

            product.Update(request.Brand, request.Description, request.Price, request.Quantity, request.SubcategoryId);

            _productRepository.Update(product);
            _uow.SaveChanges();

            return new ApiResponse<ProductDTO>
            {
                Success = true,
                NotificationType = NotificationType.Success,
                Message = SubcategoryConstants.SUBCATEGORY_SUCCESSFULLY_EDITED,
                //Data = new CategoryDTO { Id = id, Name = request.Name }
            };
        }
        catch (DomainValidationException ex)
        {
            return new ApiResponse<ProductDTO>
            {
                Success = false,
                NotificationType = NotificationType.BadRequest,
                Message = ex.Message
            };
        }
    }
}
