using backend.DTOs;
using backend.Extensions;
using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            int userId = User.GetUserId();

            var response = await categoryService.CreateCategory(request, userId);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            int userId = User.GetUserId();
            string userRole = User.GetUserRole();

            var response = await categoryService.DeletCategory(id, userId, userRole);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            int userId = User.GetUserId();
            string userRole = User.GetUserRole();

            var response = await categoryService.GetCategoryById(id, userId, userRole);

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryRequest request)
        {
            int userId = User.GetUserId();
            string userRole = User.GetUserRole();

            var response = await categoryService.UpdateCategory(id, request, userId, userRole);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetPageCategories()
        {
            int userId = User.GetUserId();
            string userRole = User.GetUserRole();

            var response = await categoryService.GetPageCategories(userId, userRole);

            return Ok(response);
        }
    }
}
