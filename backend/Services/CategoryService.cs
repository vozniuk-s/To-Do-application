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

            logger.LogInformation("Created category {Id}", newEntity.Id);

            return new CategoryResponse(newEntity.Id, newEntity.Name);
        }
        public async Task<bool> DeletCategory(int id)
        {
            var entity = await db.Categories.FirstOrDefaultAsync(u => u.Id == id);

            if(entity == null)
            {
                logger.LogWarning("Error while deleting category {id}", id);
                return false;
            }

            db.Categories.Remove(entity);
            await db.SaveChangesAsync();

            logger.LogInformation("Deleted category {Id}", id);

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
                logger.LogWarning("Problem while getting catrgory {id}", id);
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
                logger.LogWarning("Problem while getting category {name}", name);
                return null;
            }

            return response;
        }
        public async Task<CategoryResponse?> UpdateCategory(int id, UpdateCategoryRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var entity = await db.Categories.FirstOrDefaultAsync(u => u.Id == id);

            if (entity == null)
            {
                logger.LogWarning("Error while updating category {id}", id);
                return null;
            }

            entity.Name = request.Name;
            await db.SaveChangesAsync();

            logger.LogInformation("Category with ID {id} updated", id);

            return new CategoryResponse(entity.Id, entity.Name);
        }
    }
}
