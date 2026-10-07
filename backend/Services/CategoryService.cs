using backend.Data;
using backend.DTOs;
using backend.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class CategoryService(AppDbContext db, ILogger<CategoryService> logger) : ICategoryService
    {
        public async Task<CategoryResponse> CreateCategory(CreateCategoryRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var newEntity = new CategoryEntity
            {
                Name = request.Name
            };

            await db.Categories.AddAsync(newEntity);
            await db.SaveChangesAsync();

            logger.LogInformation("Category created {Id}", newEntity.Id);

            return new CategoryResponse(newEntity.Id, newEntity.Name);
        }
        public async Task<bool> DeletCategory(int id, string currentUserRole)
        {

            var entity = await GetCategoryAndValidateAccess(id, currentUserRole);

            db.Categories.Remove(entity);
            await db.SaveChangesAsync();

            logger.LogInformation("Category deleted {Id}", id);

            return true;
        }
        public async Task<List<CategoryResponse>> GetAllCategories()
        {
            return await db.Categories
                .Select(u => new CategoryResponse
                (
                    u.Id,
                    u.Name
                )).ToListAsync();
        }
        public async Task<CategoryResponse?> GetCategoryById(int id)
        {
            var response = await db.Categories
                .Where(u => u.Id == id)
                .Select(u => new CategoryResponse
                (
                    u.Id,
                    u.Name
                )).FirstOrDefaultAsync();

            if(response == null)
            {
                logger.LogWarning("Category with ID {id} was not found", id);
                return null;
            }

            return response;
        }
        public async Task<CategoryResponse?> GetCategoryByName(string name)
        {
            var response = await db.Categories
                .Where(u => u.Name == name)
                .Select(u => new CategoryResponse
                (
                    u.Id,
                    u.Name
                )).FirstOrDefaultAsync();

            if (response == null)
            {
                logger.LogWarning("Category with Name {name} was not found", name);
                return null;
            }

            return response;
        }
        public async Task<CategoryResponse?> UpdateCategory(int id, UpdateCategoryRequest request, string currentUserRole)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var entity = await GetCategoryAndValidateAccess(id, currentUserRole);

            entity.Name = request.Name;
            await db.SaveChangesAsync();

            logger.LogInformation("Category Updated {id}", id);

            return new CategoryResponse(entity.Id, entity.Name);
        }

        private async Task<CategoryEntity> GetCategoryAndValidateAccess(int categoryId, string currentUserRole)
        {
            var entity = await db.Categories
                .FirstOrDefaultAsync(u => u.Id == categoryId);

            if (entity == null)
                throw new KeyNotFoundException($"Category was not found {categoryId}");

            if (currentUserRole != "Admin")
                throw new UnauthorizedAccessException("You do not have right access");

            return entity;
        }
    }
}
