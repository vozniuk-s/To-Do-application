using backend.DTOs;

namespace backend.Interfaces
{
    public interface ICategoryService
    {
        public Task<CategoryResponse> CreateCategory(CreateCategoryRequest request, int currentUserId);
        public Task<bool> DeletCategory(int id, int currentUserId, string currentUserRole);
        public Task<CategoryResponse?> GetCategoryById(int id, int currentUserId, string currentUserRole);
        public Task<CategoryResponse?> UpdateCategory(int id, UpdateCategoryRequest request, int currentUserId, string currentUserRole);
        public Task<List<CategoryResponse>> GetPageCategories(int currentUserId, string currentUserRole);
    }
}
