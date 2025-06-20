using eShop.Main.DTOs.Category;
using eShop.Main.DTOs.Product;
using eShop.Main.Requests.Product;
using eShop.Main.Responses;

namespace eShop.Main.Interfaces;

public interface IProductService
{
    ApiResponse<List<ProductDTO>> GetProducts(ProductRequest request);
    ApiResponse<string> CreateProduct(CreateProductRequest request);
    ApiResponse<string> DeleteProduct(Guid id);
    ApiResponse<ProductDTO> GetProductById(Guid id);
}
