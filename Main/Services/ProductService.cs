using eShop.Main.DTOs.Product;
using eShop.Main.Interfaces;
using eShop.Main.Requests.Product;
using Main.Responses;

namespace eShop.Main.Services;

public class ProductService : IProductService
{
    public ApiResponse<List<ProductDTO>> GetProducts(ProductRequest request)
    {
        try
        {
            return new ApiResponse<List<ProductDTO>>
            {
                Success = true,

            };
        }
        catch (Exception ex)
        {

            return new ApiResponse<List<ProductDTO>>() 
            { 
                Success = false 
            };
        }

    }
}
