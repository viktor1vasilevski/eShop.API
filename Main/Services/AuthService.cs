using Domain.Interfaces;
using Domain.Models;
using eShop.Domain.Enums;
using eShop.Domain.Interfaces;
using eShop.Infrastructure.Data.Context;
using eShop.Main.Interfaces;
using eShop.Main.Responses;
using Main.Constants;
using Main.DTOs.Auth;
using Main.Enums;
using Main.Requests.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Main.Services
{
    public class AuthService(IUnitOfWork<AppDbContext> _uow, IConfiguration _configuration) : IAuthService
    {
        private readonly IGenericRepository<User> _userRepository = _uow.GetGenericRepository<User>();

        public async Task<ApiResponse<LoginDTO>> LoginAsync(UserLoginRequest request)
        {
            var response = await _userRepository.GetAsync(x => x.Username.ToLower() == request.Username.ToLower());
            var user = response?.FirstOrDefault();

            if (user is null || user?.Role.ToString() != Role.Admin.ToString())
            {
                return new ApiResponse<LoginDTO>
                {
                    Message = AuthConstants.USER_NOT_FOUND,
                    Success = false,
                    NotificationType = NotificationType.BadRequest
                };
            }

            if (!user.VerifyPassword(request.Password))
                return new ApiResponse<LoginDTO>
                {
                    Message = AuthConstants.INVALID_PASSWORD,
                    Success = false,
                    NotificationType = NotificationType.NotFound
                };


            var token = GenerateJwtToken(user);

            return new ApiResponse<LoginDTO>
            {
                Success = true,
                NotificationType = NotificationType.Success,
                Message = AuthConstants.ADMIN_LOGIN_SUCCESS,
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
}
