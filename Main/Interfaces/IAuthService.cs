using eShop.Main.Responses;
using Main.DTOs.Auth;
using Main.Requests.Auth;

namespace Main.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<LoginDTO>> LoginAsync(UserLoginRequest request);
}
