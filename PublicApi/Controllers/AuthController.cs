using eShop.Main.Interfaces;
using eShop.Main.Requests.Auth;
using Main.Requests.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eShop.PublicApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthController(ICustomerAuthService _customerAuthService) : BaseController
{

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] UserLoginRequest request)
    {
        var response = await _customerAuthService.LoginAsync(request);
        return HandleResponse(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] UserRegisterRequest request)
    {
        var response = await _customerAuthService.RegisterAsync(request);
        return HandleResponse(response);
    }
}
