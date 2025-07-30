using Main.Requests.Auth;

namespace Admin.eShop.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthController(IAuthService _authService) : BaseController
{

    [HttpPost("admin/login")]
    public async Task<IActionResult> LoginAsync([FromBody] UserLoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        return HandleResponse(response);
    }
}
