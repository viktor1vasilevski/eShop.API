using eShop.Main.Interfaces;
using eShop.Main.Requests.Cart;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eShop.PublicApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Customer")]
    public class BasketController(IBasketService _basketService) : BaseController
    {


        [HttpPost("Merge/{userId}")]
        public async Task<IActionResult> Merge([FromRoute] Guid userId, [FromBody] List<BasketRequest> request)
        {
            var response = await _basketService.Merge(userId, request);
            return HandleResponse(response);
        }
    }
}
