using backend.DTOs;
using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using backend.Extensions;

namespace backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController(ITaskService taskService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody]CreateTaskRequest request)
        {
            int userId = User.GetUserId();

            var response = await taskService.CreateTask(request, userId);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            int userId = User.GetUserId();
            string userRole = User.GetUserRole();

            var response = await taskService.DeleteTask(id, userId, userRole);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            int userId = User.GetUserId();
            string userRole = User.GetUserRole();

            var response = await taskService.GetTaskById(id, userId, userRole);

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody]UpdateTaskRequest request)
        {
            int userId = User.GetUserId();
            string userRole = User.GetUserRole();

            var response = await taskService.UpdateTask(id, request, userId, userRole);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetPageTasks(
            [FromQuery] string? searchString,
            [FromQuery] int? categoryId,
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10)
        {
            int userId = User.GetUserId();
            string userRole = User.GetUserRole();

            var response = await taskService.GetPageTasks(searchString, categoryId, pageNumber, pageSize, userId, userRole);

            return Ok(response);
        }
    }
}
