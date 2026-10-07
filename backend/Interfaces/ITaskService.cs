using backend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace backend.Interfaces
{
    public interface ITaskService
    {
        public Task<TaskResponse> CreateTask(CreateTaskRequest request, int currentUserId);
        public Task<bool> DeleteTask(int id, int currentUserId, string currentUserRole);
        public Task<TaskResponse?> GetTaskById(int id, int currentUserId, string currentUserRole);
        public Task<TaskResponse?> UpdateTask(int id, UpdateTaskRequest request, int currentUserId, string currentUserRole);
        public Task<List<TaskResponse>> GetPageTasks(string? searchString, int? categoryId, int pageNumber, int pageSize, int currentUserId, string currentUserRole);
    }
}
