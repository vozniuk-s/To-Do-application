using backend.DTOs;

namespace backend.Interfaces
{
    public interface ICategoryService
    {
        public Task<CategoryResponse> CreateCategory(CreateCategoryRequest request);
        public Task<bool> DeletCategory(int id);
        public Task<List<CategoryResponse>> GetAllCategories();
        public Task<CategoryResponse?> GetCategoryById(int id);
        public Task<CategoryResponse?> GetCategoryByName(string name);
        public Task<CategoryResponse?> UpdateCategory(int id, UpdateCategoryRequest request);
    }
}
