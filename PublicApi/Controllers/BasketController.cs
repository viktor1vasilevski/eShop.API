using eShop.Main.Interfaces;
using eShop.Main.Requests.Cart;
using Microsoft.AspNetCore.Mvc;

namespace eShop.PublicApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController(IBasketService _basketService) : BaseController
    {

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetBasketByUserId([FromRoute] Guid userId)
        {
            var response = await _basketService.GetBasketByUserIdAsync(userId);
            return HandleResponse(response);
        }

        [HttpPost("Merge/{userId}")]
        public async Task<IActionResult> Merge([FromRoute] Guid userId, [FromBody] List<BasketRequest> request)
        {
            var response = await _basketService.Merge(userId, request);
            return HandleResponse(response);
        }
    }
}
