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
        public async Task<TaskResponse> CreateTask(CreateTaskRequest request, int currentUserId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var newEntity = new TaskEntity
            {
                Name = request.Name,
                Description = request.Description,
                CategoryId = request.CategoryId,
                UserId = currentUserId,
            };

            await db.Tasks.AddAsync(newEntity);
            await db.SaveChangesAsync();

            logger.LogInformation("Task created {Id}", newEntity.Id);

            return new TaskResponse(newEntity.Id, newEntity.Name, newEntity.Description, null, newEntity.UserId);
        }

        public async Task<bool> DeleteTask(int id, int currentUserId, string currentUserRole)
        {
            var entity = await GetTaskAndValidateAccess(id, currentUserId, currentUserRole);

            db.Tasks.Remove(entity);
            await db.SaveChangesAsync();

            logger.LogInformation("Task deleted {Id}", id);

            return true;
        }

        public async Task<TaskResponse?> GetTaskById(int id, int currentUserId, string currentUserRole)
        {
            var entity = await GetTaskAndValidateAccess(id, currentUserId, currentUserRole);

            return new TaskResponse(
                entity.Id, 
                entity.Name, 
                entity.Description,
                entity.Category != null ? new CategoryResponse(entity.Category.Id, entity.Category.Name) : null, 
                entity.UserId);
        }

        public async Task<TaskResponse?> UpdateTask(int id, UpdateTaskRequest request, int currentUserId, string currentUserRole)
        {
            if(request == null)
                throw new ArgumentNullException(nameof(request));

            var entity = await GetTaskAndValidateAccess(id, currentUserId, currentUserRole);

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

            logger.LogInformation("Task updated {id}", id);

            return new TaskResponse(entity.Id, entity.Name, entity.Description, null, entity.UserId);
        }

        public async Task<List<TaskResponse>> GetPageTasks(string? searchString, int? categoryId, int pageNumber, int pageSize, int currentUserId, string currentUserRole)
        {
            var query = db.Tasks.AsQueryable();
            
            if (categoryId.HasValue)
                query = query.Where(u => u.CategoryId == categoryId.Value);

            if (!string.IsNullOrEmpty(searchString))
                query = query.Where(u => u.Name.ToLower().Contains(searchString.ToLower()));

            if(currentUserRole != "Admin")
                query = query.Where(u=> u.UserId == currentUserId);

            var response = await query
                .OrderByDescending(u => u.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new TaskResponse(u.Id, u.Name, u.Description,
                    u.Category != null ? new CategoryResponse(u.Category.Id, u.Category.Name) : null, 
                    u.UserId))
                .ToListAsync();

            return response;
        }

        private async Task<TaskEntity> GetTaskAndValidateAccess(int taskId, int currentUserId, string currentUserRole)
        {
            var entity = await db.Tasks
                .Include(u => u.Category)
                .FirstOrDefaultAsync(u => u.Id == taskId);

            if (entity == null)
                throw new KeyNotFoundException($"Task was not found {taskId}");

            if (currentUserRole != "Admin" && entity.UserId != currentUserId)
                throw new UnauthorizedAccessException("You do not have right access");

            return entity;
        }
    }
}
