using backend.DTOs;

namespace backend.Interfaces
{
    public interface ITaskService
    {
        public Task<TaskResponse> CreateTask(CreateTaskRequest request);
        public Task<bool> DeleteTask(int id);
        public Task<List<TaskResponse>> GetAllTasks();
        public Task<TaskResponse?> GetTaskById(int id);
    }
}
