using Domain.Interfaces;
using Domain.Models;
using eShop.Domain.Interfaces;
using eShop.Infrastructure.Data.Context;
using eShop.Main.DTOs.Category;
using eShop.Main.DTOs.User;
using eShop.Main.Interfaces;
using eShop.Main.Responses;
using Main.Enums;
using Microsoft.Extensions.Logging;

namespace eShop.Main.Services;

public class UserService(IUnitOfWork<AppDbContext> _uow, ILogger<CategoryService> _logger) : IUserService
{
    private readonly IGenericRepository<User> _userRepository = _uow.GetGenericRepository<User>();


    public ApiResponse<List<UserDTO>> GetUsers()
    {
        var users = _userRepository.GetAsQueryable();

        var usersDTO = users.Select(x => new UserDTO
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Email = x.Email,
            Role = x.Role.ToString(),
        }).ToList();

        return new ApiResponse<List<UserDTO>>
        {
            NotificationType = NotificationType.Success,
            Data = usersDTO
        };
    }
}
