using Admin.eShop.Controllers;
using eShop.Main.Interfaces;
using eShop.Main.Requests.Category;
using eShop.Main.Requests.Subcategory;
using eShop.Main.Services;
using Main.Enums;
using Main.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Admin.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class SubcategoryController(ISubcategoryService subcategoryService) : BaseController
    {
        private readonly ISubcategoryService _subcategoryService = subcategoryService;


        [HttpGet]
        public IActionResult Get([FromQuery] SubcategoryRequest request)
        {
            var response = _subcategoryService.GetSubcategories(request);
            return HandleResponse(response);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var response = _subcategoryService.GetSubcategoryById(id);
            return HandleResponse(response);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateSubcategoryRequest request)
        {
            var response = _subcategoryService.CreateSubcategory(request);
            return HandleResponse(response);
        }

        [HttpPut("{id}")]
        public IActionResult Edit([FromRoute] Guid id, [FromBody] EditSubcategoryRequest request)
        {
            var response = _subcategoryService.EditSubcategory(id, request);
            return HandleResponse(response);
        }


        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] Guid id)
        {
            var response = _subcategoryService.DeleteSubcategory(id);
            return HandleResponse(response);
        }
    }
}
