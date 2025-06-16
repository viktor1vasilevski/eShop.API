using eShop.Main.DTOs.Product;
using eShop.Main.Requests.Product;
using Main.Responses;

namespace eShop.Main.Interfaces;

public interface IProductService
{
    ApiResponse<List<ProductDTO>> GetProducts(ProductRequest request);
}
