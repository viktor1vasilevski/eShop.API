using Domain.Interfaces;
using Domain.Models;
using eShop.Domain.Exceptions;
using eShop.Domain.Interfaces;
using eShop.Infrastructure.Data.Context;
using eShop.Main.Constants;
using eShop.Main.DTOs.Product;
using eShop.Main.Interfaces;
using eShop.Main.Requests.Product;
using eShop.Main.Responses;
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
        var products = _productRepository.GetAsQueryableWhereIf(x =>
                x.WhereIf(!String.IsNullOrEmpty(request.CategoryId.ToString()), x => x.Subcategory.Category.Id == request.CategoryId)
                 .WhereIf(!String.IsNullOrEmpty(request.SubcategoryId.ToString()), x => x.Subcategory.Id == request.SubcategoryId)
                 .WhereIf(!String.IsNullOrEmpty(request.Description), x => x.Description.ToLower().Contains(request.Description.ToLower()))
                 .WhereIf(!String.IsNullOrEmpty(request.Name), x => x.Name.ToLower().Contains(request.Name.ToLower())),
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
            Name = x.Name,
            Description = x.Description,
            UnitPrice = x.UnitPrice,
            UnitQuantity = x.UnitQuantity,
            Image = x.Image != null ? $"data:{x.ImageType};base64,{Convert.ToBase64String(x.Image)}" : null,
            Category = x.Subcategory.Category.Name,
            Subcategory = x.Subcategory.Name,
            SubcategoryId = x.SubcategoryId,
            Created = x.Created,
            LastModified = x.LastModified,
        }).ToList();

        return new ApiResponse<List<ProductDTO>>()
        {
            Data = productsDTO,
            TotalCount = totalCount,
            NotificationType = NotificationType.Success
        };
    }

    public ApiResponse<ProductDTO> CreateProduct(CreateProductRequest request)
    {
        if (!_subcategoryRepository.Exists(x => x.Id == request.SubcategoryId))
            return new ApiResponse<ProductDTO>
            {
                Message = SubcategoryConstants.SUBCATEGORY_DOESNT_EXIST,
                NotificationType = NotificationType.NotFound
            };

        try
        {
            var productData = new ProductData(
                name: request.Name,
                description: request.Description,
                unitPrice: request.Price,
                unitQuantity: request.Quantity,
                subcategoryId: request.SubcategoryId,
                base64Image: request.Image);

            var product = new Product(productData);
            _productRepository.Insert(product);
            _uow.SaveChanges();

            return new ApiResponse<ProductDTO>
            {
                Message = ProductConstants.PRODUCT_SUCCESSFULLY_CREATED,
                NotificationType = NotificationType.Created,
                Data = new ProductDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    UnitPrice = product.UnitPrice,
                    UnitQuantity = product.UnitQuantity,
                    SubcategoryId = product.SubcategoryId,
                    Created = product.Created,
                }
            };
        }
        catch (DomainValidationException ex)
        {
            return new ApiResponse<ProductDTO>
            {
                NotificationType = NotificationType.BadRequest,
                Message = ex.Message
            };
        }
    }

    public ApiResponse<string> DeleteProduct(Guid id)
    {
        var product = _productRepository.GetById(id);
        if (product is null)
            return new ApiResponse<string>
            {
                NotificationType = NotificationType.NotFound,
                Message = ProductConstants.PRODUCT_DOESNT_EXIST
            };

        _productRepository.Delete(product);
        _uow.SaveChanges();

        return new ApiResponse<string>
        {
            Message = ProductConstants.PRODUCT_SUCCESSFULLY_DELETED,
            NotificationType = NotificationType.Success
        };
    }

    public ApiResponse<ProductDTO> GetProductById(Guid id)
    {
        var product = _productRepository.Get(
                filter: x => x.Id == id,
                include: x => x.Include(x => x.Subcategory).ThenInclude(x => x.Category)).FirstOrDefault();

        if (product is null)
            return new ApiResponse<ProductDTO>
            {
                NotificationType = NotificationType.NotFound,
                Message = ProductConstants.PRODUCT_DOESNT_EXIST
            };

        var productDto = new ProductDTO()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            UnitPrice = product.UnitPrice,
            UnitQuantity = product.UnitQuantity,
            SubcategoryId = product.SubcategoryId,
            Subcategory = product.Subcategory?.Name,
            Category = product.Subcategory?.Category?.Name,
            LastModified = product.LastModified,
            Created = product.Created,
            Image = product.Image != null ? $"data:{product.ImageType};base64,{Convert.ToBase64String(product.Image)}" : null,
        };


        return new ApiResponse<ProductDTO>
        {
            NotificationType = NotificationType.Success,
            Data = productDto
        };
    }

    public ApiResponse<ProductDTO> EditProduct(Guid id, EditProductRequest request)
    {
        var product = _productRepository.GetById(id);
        if (product is null)
            return new ApiResponse<ProductDTO>
            {
                NotificationType = NotificationType.NotFound,
                Message = ProductConstants.PRODUCT_DOESNT_EXIST
            };

        if (_productRepository.Exists(x => x.Name.ToLower() == request.Name.ToLower() && x.Id != id))
            return new ApiResponse<ProductDTO>
            {
                NotificationType = NotificationType.Conflict,
                Message = ProductConstants.PRODUCT_EXISTS
            };

        if (!_subcategoryRepository.Exists(x => x.Id == request.SubcategoryId))
            return new ApiResponse<ProductDTO>
            {
                NotificationType = NotificationType.NotFound,
                Message = SubcategoryConstants.SUBCATEGORY_DOESNT_EXIST,
            };

        try
        {
            var productData = new ProductData(
                name: request.Name,
                description: request.Description,
                unitPrice: request.Price,
                unitQuantity: request.Quantity,
                subcategoryId: request.SubcategoryId,
                base64Image: request.Image);

            product.Update(productData);
            _productRepository.Update(product);
            _uow.SaveChanges();

            return new ApiResponse<ProductDTO>
            {
                NotificationType = NotificationType.Success,
                Message = ProductConstants.PRODUCT_SUCCESSFULLY_UPDATED,
                //Data = new CategoryDTO { Id = id, Name = request.Name }
            };
        }
        catch (DomainValidationException ex)
        {
            return new ApiResponse<ProductDTO>
            {
                NotificationType = NotificationType.BadRequest,
                Message = ex.Message
            };
        }
    }
}
