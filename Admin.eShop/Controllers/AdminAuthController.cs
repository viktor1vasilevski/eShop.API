using Main.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Main.Requests.Auth;
using Microsoft.AspNetCore.Authorization;

namespace Admin.eShop.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AdminAuthController(IAdminAuthService authService) : BaseController
{
    private readonly IAdminAuthService _adminAuthService = authService;

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] UserLoginRequest request)
    {
        var response = await _adminAuthService.AdminLoginAsync(request);
        return HandleResponse(response);
    }
}
