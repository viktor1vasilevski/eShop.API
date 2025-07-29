using Domain.Interfaces;
using Domain.Models;
using eShop.Domain.Enums;
using eShop.Domain.Exceptions;
using eShop.Domain.Interfaces;
using eShop.Infrastructure.Data.Context;
using eShop.Main.DTOs.Auth;
using eShop.Main.Interfaces;
using eShop.Main.Requests.Auth;
using eShop.Main.Responses;
using Main.Constants;
using Main.DTOs.Auth;
using Main.Enums;
using Main.Requests.Auth;
using Main.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
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
                NotificationType = NotificationType.NotFound
            };
        }

        if (!user.VerifyPassword(request.Password))
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
        var usersExist = await _userRepository.ExistsAsync(
            filter: x => x.Username.ToLower() == request.Username.ToLower() || 
                         x.Email.ToLower() == request.Email.ToLower());

        if (usersExist)
            return new ApiResponse<RegisterDTO>
            {
                Success = false,
                NotificationType = NotificationType.Conflict,
                Message = AuthConstants.ACCOUNT_ALREADY_EXISTS
            };

        try
        {
            var user = User.CreateNew(request.FirstName, request.LastName, 
                request.Username, request.Email, request.Password, Role.Customer);

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
        catch (DomainValidationException ex)
        {
            return new ApiResponse<RegisterDTO>
            {
                Success = false,
                NotificationType = NotificationType.BadRequest,
                Message = ex.Message
            };
        }

    }

    private string GenerateJwtToken(User user)
    {
        var secretKey = _configuration["JwtSettings:Secret"] ?? "AlternativeSecretKeyOfAtLeast32Characters!";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(ClaimTypes.Name, user.Username)
            };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(22),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
