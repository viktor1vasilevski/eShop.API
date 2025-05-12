using Main.DTOs.Auth;
using Main.Requests.Auth;
using Main.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Interfaces
{
    public interface IAdminAuthService
    {
        Task<ApiResponse<LoginDTO>> AdminLoginAsync(UserLoginRequest request);
    }
}
