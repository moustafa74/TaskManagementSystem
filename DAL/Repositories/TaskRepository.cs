using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Api.Data;
using TaskManagementSystem.Models;
using TaskStatus = TaskManagementSystem.Models.TaskStatus;

namespace TaskManagementSystem.DAL.Repositories
{
    public class TaskRepository : GenericRepository<TaskEntity>, ITaskRepository 
    {
        public TaskRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TaskEntity>> GetTasksByUserIdAsync(string userId)
        {
            return await _context.TaskEntities 
                .Include(t => t.TaskAssignments)
                .Where(t => t.TaskAssignments.Any(ta => ta.UserId == userId))
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskEntity>> GetTasksByTeamIdAsync(int teamId)
        {
            return await _context.TaskEntities 
                .Include(t => t.TaskAssignments)
                .Where(t => t.TaskAssignments.Any(ta => ta.TeamId == teamId))
                .ToListAsync();
        }
        public async Task<IEnumerable<TaskEntity>> GetOpenTasksAsync()
        {
            return await _dbSet.Where(t => t.Status == TaskStatus.ToDo || t.Status == TaskStatus.InProgress).ToListAsync();
        }

        public async Task<IEnumerable<TaskEntity>> GetTasksByPriorityAsync(PriorityLevel priority)
        {
            return await _dbSet.Where(t => t.Priority == priority).ToListAsync();
        }
        public async Task AddTaskAssignmentAsync(TaskAssignment taskAssignment)
        {
            await _context.TaskAssignments.AddAsync(taskAssignment);
        }
    }
}
