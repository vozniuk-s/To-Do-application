using backend.Data;
using backend.Interfaces;
using backend.Models;
using backend.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Forms;

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
                logger.LogWarning("Error while deleting task {id}", id);
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
            var response = await db.Tasks
                .Where(u => u.Id == id)
                .Select(u => new TaskResponse(
                    u.Id,
                    u.Name,
                    u.Description,
                    u.Category != null ? new CategoryResponse(u.Category.Id, u.Category.Name) : null
                )).FirstOrDefaultAsync();

            if (response == null)
            {
                logger.LogWarning("Problem while getting task {id}", id);
                return null;
            }

            return response;
        }

        public async Task<TaskResponse?> UpdateTask(int id, UpdateTaskRequest request)
        {
            if(request == null)
                throw new ArgumentNullException(nameof(request));

            var entity = await db.Tasks.FirstOrDefaultAsync(u => u.Id == id);

            if(entity == null)
            {
                logger.LogWarning("Error while updating task {id}", id);
                return null;
            }

            if (request.CategoryId.HasValue)
            {
                var exist = await db.Categories.AnyAsync(u => u.Id == request.CategoryId.Value);

                if (!exist)
                    throw new ArgumentException($"Category with ID {request.CategoryId.Value} does not exist");
            }

            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.CategoryId = request.CategoryId;

            await db.SaveChangesAsync();

            logger.LogInformation("Task with ID{id} updated", id);

            return new TaskResponse(entity.Id, entity.Name, entity.Description, null);
        }

        public async Task<List<TaskResponse>> GetPageTasks(string? searchString, int? categoryId, int pageNumber, int pageSize)
        {
            var query = db.Tasks.AsQueryable();
            
            if (categoryId.HasValue)
                query = query.Where(u => u.CategoryId == categoryId.Value);

            if (!string.IsNullOrEmpty(searchString))
                query = query.Where(u => u.Name.ToLower().Contains(searchString.ToLower()));

            var response = await query
                .OrderByDescending(u => u.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new TaskResponse(u.Id, u.Name, u.Description,
                    u.Category != null ? new CategoryResponse(u.Category.Id, u.Category.Name) : null))
                .ToListAsync();

            return response;
        }
    }
}
