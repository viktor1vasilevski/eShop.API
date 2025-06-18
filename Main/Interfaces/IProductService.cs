using eShop.Main.DTOs.Product;
using eShop.Main.Requests.Product;
using eShop.Main.Requests.Subcategory;
using eShop.Main.Responses;

namespace eShop.Main.Interfaces;

public interface IProductService
{
    ApiResponse<List<ProductDTO>> GetProducts(ProductRequest request);
    ApiResponse<string> CreateProduct(CreateProductRequest request);
}
