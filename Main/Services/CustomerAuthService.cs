using Domain.Interfaces;
using Domain.Models;
using eShop.Domain.Enums;
using eShop.Main.DTOs.Auth;
using eShop.Main.Helpers;
using eShop.Main.Interfaces;
using eShop.Main.Requests.Auth;
using eShop.Main.Responses;
using Infrastructure.Data.Context;
using Main.Constants;
using Main.DTOs.Auth;
using Main.Enums;
using Main.Requests.Auth;
using Main.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace eShop.Main.Services;

public class CustomerAuthService(IUnitOfWork<AppDbContext> _uow,
        IConfiguration _configuration,
        ILogger<AuthService> _logger) : ICustomerAuthService
{
    private readonly IGenericRepository<User> _userRepository = _uow.GetGenericRepository<User>();
    public async Task<ApiResponse<LoginDTO>> LoginAsync(UserLoginRequest request)
    {
        var response = await _userRepository.GetAsync(x => x.Username.ToLower() == request.Username.ToLower());
        var user = response?.FirstOrDefault();

        if (user is null || user?.Role.ToString() != Role.Customer.ToString())
        {
            return new ApiResponse<LoginDTO>
            {
                Message = AuthConstants.USER_NOT_FOUND,
                Success = false,
                NotificationType = NotificationType.BadRequest
            };
        }

        var isPasswordValid = PasswordHasher.VerifyPassword(request.Password, user.PasswordHash, user.SaltKey);

        if (!isPasswordValid)
        {
            return new ApiResponse<LoginDTO>
            {
                Message = AuthConstants.INVALID_PASSWORD,
                Success = false,
                NotificationType = NotificationType.BadRequest
            };
        }

        var token = GenerateJwtToken(user);

        return new ApiResponse<LoginDTO>
        {
            Success = true,
            NotificationType = NotificationType.Success,
            Message = AuthConstants.CUSTOMER_LOGIN_SUCCESS,
            Data = new LoginDTO
            {
                Id = user.Id,
                Token = token,
                Email = user.Email,
                Username = user.Username,
                Role = user.Role.ToString()
            }
        };
    }

    public async Task<ApiResponse<RegisterDTO>> RegisterAsync(UserRegisterRequest request)
    {
        var users = await _userRepository.GetAsync(
            filter: x => x.Username.ToLower() == request.Username.ToLower() && 
                         x.Email.ToLower() == request.Email.ToLower());

        if (users.FirstOrDefault() is not null)
            return new ApiResponse<RegisterDTO>
            {
                Success = false,
                NotificationType = NotificationType.Conflict,
                Message = AuthConstants.ACCOUNT_ALREADY_EXISTS
            };

        var saltKey = PasswordHasher.GenerateSalt();
        var user = new User();
        user.Username = request.Username;
        user.Email = request.Email;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Role = Role.Customer;
        user.PasswordHash = PasswordHasher.HashPassword(request.Password, saltKey);
        user.SaltKey = Convert.ToBase64String(saltKey);

        await _userRepository.InsertAsync(user);
        await _uow.SaveChangesAsync();

        return new ApiResponse<RegisterDTO>
        {
            Success = true,
            NotificationType = NotificationType.Success,
            Message = AuthConstants.CUSTOMER_REGISTER_SUCCESS,
            Data = new RegisterDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Email = user.Email
            }
        };
    }

    private string GenerateJwtToken(User user)
    {
        var secretKey = _configuration["JwtSettings:Secret"] ?? "AlternativeSecretKeyOfAtLeast32Characters!";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[] { new Claim(ClaimTypes.Role, user.Role.ToString()) };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(22),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
