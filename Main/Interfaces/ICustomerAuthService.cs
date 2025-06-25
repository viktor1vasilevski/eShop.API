using eShop.Main.DTOs.Auth;
using eShop.Main.Requests.Auth;
using eShop.Main.Responses;
using Main.Interfaces;

namespace eShop.Main.Interfaces;

public interface ICustomerAuthService : IAuthService
{
    Task<ApiResponse<RegisterDTO>> RegisterAsync(UserRegisterRequest request);
}
