using eShop.Main.DTOs.User;
using eShop.Main.Responses;

namespace eShop.Main.Interfaces;

public interface IUserService
{
    ApiResponse<List<UserDTO>> GetUsers();
}
