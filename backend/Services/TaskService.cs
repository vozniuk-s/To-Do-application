using backend.Data;
using backend.Interfaces;
using backend.Models;
using backend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class TaskService(AppDbContext db, ILogger<TaskService> logger) : ITaskService
    {
        public async Task<TaskResponse> CreateTask(CreateTaskRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var newEntity = new TaskEntity
            {
                Name = request.Name,
                Description = request.Description,
                CategoryId = request.CategoryId,
            };

            await db.Tasks.AddAsync(newEntity);
            await db.SaveChangesAsync();

            logger.LogInformation("Created task {Id}", newEntity.Id);

            return new TaskResponse(newEntity.Id, newEntity.Name, newEntity.Description, null);
        }

        public async Task<bool> DeleteTask(int id)
        {
            var taskEntity = await db.Tasks.FirstOrDefaultAsync(u => u.Id == id);

            if (taskEntity == null)
            {
                logger.LogWarning("");
                return false;
            }

            db.Tasks.Remove(taskEntity);
            await db.SaveChangesAsync();

            logger.LogInformation("Deleted task {Id}", id);

            return true;
        }

        public async Task<List<TaskResponse>> GetAllTasks()
        {
            return await db.Tasks
                .Select(u => new TaskResponse(
                    u.Id,
                    u.Name,
                    u.Description,
                    u.Category != null ? new CategoryResponse(u.Category.Id, u.Category.Name) : null
                )).ToListAsync();
        }

        public async Task<TaskResponse?> GetTaskById(int id)
        {
            return await db.Tasks
                .Where(u => u.Id == id)
                .Select(u => new TaskResponse(
                    u.Id,
                    u.Name,
                    u.Description,
                    u.Category != null ? new CategoryResponse(u.Category.Id, u.Category.Name) : null
                )).FirstOrDefaultAsync();
        }
    }
}
