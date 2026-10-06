using Microsoft.AspNetCore.Mvc;
using backend.Interfaces;
using backend.DTOs;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController(ITaskService taskService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody]CreateTaskRequest request)
        {
            var response = await taskService.CreateTask(request);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var response = await taskService.DeleteTask(id);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            var response = await taskService.GetAllTasks();

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var response = await taskService.GetTaskById(id);

            if (response == null)
                return NotFound(new { message = $"Task with ID {id} wat not found." });

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody]UpdateTaskRequest request)
        {
            var response = await taskService.UpdateTask(id, request);

            if(response == null)
                return NotFound(new { message = $"Task with ID {id} wat not found." });

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetPageTasks(
            [FromQuery] string? searchString,
            [FromQuery] int? categoryId,
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10)
        {
           var response = await taskService.GetPageTasks(searchString, categoryId, pageNumber, pageSize);

            return Ok(response);
        }
    }
}
