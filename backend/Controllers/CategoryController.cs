using Microsoft.AspNetCore.Mvc;
using backend.Interfaces;
using backend.DTOs;


namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            var response = await categoryService.CreateCategory(request);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var response = await categoryService.DeletCategory(id);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var response = await categoryService.GetAllCategories();

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var response = await categoryService.GetCategoryById(id);

            if(response == null)
                return NotFound(new {message = $"Category with ID {id} wat not found."});

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryRequest request)
        {
            var response = await categoryService.UpdateCategory(id, request);

            if(response == null)
                return NotFound(new { message = $"Category with ID {id} wat not found." });

            return Ok(response);
        }
    }
}
