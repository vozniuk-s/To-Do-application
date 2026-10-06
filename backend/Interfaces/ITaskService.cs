using backend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace backend.Interfaces
{
    public interface ITaskService
    {
        public Task<TaskResponse> CreateTask(CreateTaskRequest request);
        public Task<bool> DeleteTask(int id);
        public Task<List<TaskResponse>> GetAllTasks();
        public Task<TaskResponse?> GetTaskById(int id);
        public Task<TaskResponse?> UpdateTask(int id, UpdateTaskRequest request);
        public Task<List<TaskResponse>> GetPageTasks(string? searchString, int? categoryId, int pageNumber, int pageSize);
    }
}
