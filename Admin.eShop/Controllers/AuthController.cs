using Main.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Main.Requests.Auth;
using Microsoft.AspNetCore.Authorization;

namespace Admin.eShop.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthController(IAuthService authService) : BaseController
{
    private readonly IAuthService _authService = authService;

    [HttpPost("admin/login")]
    public async Task<IActionResult> LoginAsync([FromBody] UserLoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        return HandleResponse(response);
    }
}
