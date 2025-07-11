namespace eShop.Admin.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
public class UserController(IUserService userService) : BaseController
{
    private readonly IUserService _userService = userService;

    [HttpGet]
    public IActionResult Get()
    {
        var response = _userService.GetUsers();
        return HandleResponse(response);
    }
}
