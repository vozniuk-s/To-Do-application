using backend.Data;
using backend.DTOs;
using backend.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class CategoryService(AppDbContext db, ILogger<CategoryService> logger) : ICategoryService
    {
        public async Task<CategoryResponse> CreateCategory(CreateCategoryRequest request, int currentUserId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var newEntity = new CategoryEntity
            {
                Name = request.Name,
                UserId = currentUserId
            };

            await db.Categories.AddAsync(newEntity);
            await db.SaveChangesAsync();

            logger.LogInformation("Category created {Id}", newEntity.Id);

            return new CategoryResponse(newEntity.Id, newEntity.Name);
        }
        public async Task<bool> DeletCategory(int id, int currentUserId, string currentUserRole)
        {
            var entity = await GetCategoryAndValidateAccess(id, currentUserId, currentUserRole);

            await db.Tasks
                .Where(u => u.CategoryId == id)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.CategoryId, (int?)null));

            db.Categories.Remove(entity);
            await db.SaveChangesAsync();

            logger.LogInformation("Category deleted {Id}", id);

            return true;
        }
        public async Task<CategoryResponse?> GetCategoryById(int id, int currentUserId, string currentUserRole)
        {
            var entity = await GetCategoryAndValidateAccess(id, currentUserId, currentUserRole);

            return new CategoryResponse(entity.Id, entity.Name);
        }
        public async Task<CategoryResponse?> UpdateCategory(int id, UpdateCategoryRequest request, int currentUserId, string currentUserRole)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var entity = await GetCategoryAndValidateAccess(id, currentUserId, currentUserRole);

            entity.Name = request.Name;
            await db.SaveChangesAsync();

            logger.LogInformation("Category Updated {id}", id);

            return new CategoryResponse(entity.Id, entity.Name);
        }
        public async Task<List<CategoryResponse>> GetPageCategories(int currentUserId, string currentUserRole)
        {
            var query = db.Categories.AsQueryable();

            if (currentUserRole != "Admin")
                query = query.Where(u => u.UserId == currentUserId);

            var response = await query
                .Select(u => new CategoryResponse(u.Id, u.Name))
                .ToListAsync();

            return response;
        }

        private async Task<CategoryEntity> GetCategoryAndValidateAccess(int categoryId, int currentUserId, string currentUserRole)
        {
            var entity = await db.Categories
                .FirstOrDefaultAsync(u => u.Id == categoryId);

            if (entity == null)
                throw new KeyNotFoundException($"Category was not found {categoryId}");

            if (currentUserRole != "Admin" && currentUserId != entity.UserId)
                throw new UnauthorizedAccessException("You do not have right access");

            return entity;
        }
    }
}
