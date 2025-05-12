using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Data.Context;
using Main.DTOs.Auth;
using Main.Enums;
using Main.Helpers;
using Main.Interfaces;
using Main.Requests.Auth;
using Main.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Main.Services
{
    public class AdminAuthService(IUnitOfWork<AppDbContext> _uow, 
        IConfiguration _configuration, 
        ILogger<AdminAuthService> _logger) : IAdminAuthService
    {
        private readonly IGenericRepository<User> _userRepository = _uow.GetGenericRepository<User>();

        public async Task<ApiResponse<LoginDTO>> UserLoginAsync(UserLoginRequest request)
        {
            try
            {
                var response = await _userRepository.GetAsync(x => x.Username.ToLower() == request.Username.ToLower());
                var user = response?.FirstOrDefault();

                if (user is null)
                {
                    return new ApiResponse<LoginDTO>
                    {
                        Message = "user not found",
                        Success = false,
                        NotificationType = NotificationType.BadRequest
                    };
                }

                var isPasswordValid = PasswordHasher.VerifyPassword(request.Password, user.PasswordHash, user.SaltKey);

                if (!isPasswordValid)
                {
                    return new ApiResponse<LoginDTO>
                    {
                        Message = "invalid pass",
                        Success = false,
                        NotificationType = NotificationType.BadRequest
                    };
                }

                var token = GenerateJwtToken(user);

                return new ApiResponse<LoginDTO>
                {
                    Success = true,
                    NotificationType = NotificationType.Success,
                    Message = "Success",
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loggin user at {Timestamp}. Username: {Username}",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), request.Username);

                return new ApiResponse<LoginDTO>
                {
                    Success = false,
                    NotificationType = NotificationType.ServerError,
                    Message = "1"
                };
            }
        }

        private string GenerateJwtToken(User user)
        {
            var secretKey = _configuration["JwtSettings:Secret"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.Role, user.Role.ToString()),
        };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(22),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
