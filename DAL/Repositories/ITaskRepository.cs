using TaskManagementSystem.Models;

namespace TaskManagementSystem.DAL.Repositories
{
    public interface ITaskRepository : IGenericRepository<TaskEntity>
    {
        Task<IEnumerable<TaskEntity>> GetTasksByUserIdAsync(string userId);
        Task<IEnumerable<TaskEntity>> GetOpenTasksAsync();
        Task<IEnumerable<TaskEntity>> GetTasksByPriorityAsync(PriorityLevel priority);
        Task<IEnumerable<TaskEntity>> GetTasksByTeamIdAsync(int teamId);
        Task AddTaskAssignmentAsync(TaskAssignment taskAssignment);
    }
}
