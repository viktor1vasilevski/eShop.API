using eShop.Main.Interfaces;
using Main.Requests.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eShop.PublicApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthController(ICustomerAuthService customerAuthService) : BaseController
{
    private readonly ICustomerAuthService _customerAuthService = customerAuthService;

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] UserLoginRequest request)
    {
        var response = await _customerAuthService.LoginAsync(request);
        return HandleResponse(response);
    }
}
