using Microsoft.AspNetCore.Identity;
using TaskManagementSystem.DAL.Repositories;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.BLL
{
    public class TaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        string user;


        public TaskService(ITaskRepository taskRepository, UserManager<ApplicationUser> userManager)
        {
            _taskRepository = taskRepository;
            _userManager = userManager;
             
        }
        
        public async Task<IEnumerable<TaskEntity>> GetAllTasksAsync()
        {
            return await _taskRepository.GetAllAsync();
        }
        //public async Task<bool> IsTeamLeadAuthorizedForTask(string teamLeadId, int taskId)
        //{
        //    var task = await _taskRepository.GetByIdAsync(taskId);
        //    if (task == null)
        //    {
        //        return false;
        //    }

        //    // تحقق من أن المستخدم هو TeamLead لفريق مرتبط بالمهمة
        //    var isTeamLead = task.TaskAssignments.Any(ta => ta.Team.TeamLeadId == teamLeadId);
        //    return isTeamLead;
        //}

        public async Task<IEnumerable<TaskEntity>> GetTasksByUserIdAsync(string userId)
        {
            return await _taskRepository.GetTasksByUserIdAsync(userId);
        }

        public async Task<IEnumerable<TaskEntity>> GetOpenTasksAsync()
        {
            return await _taskRepository.GetOpenTasksAsync();
        }

        public async Task<TaskEntity> GetTaskByIdAsync(int id)
        {
            return await _taskRepository.GetByIdAsync(id);
        }

        public async Task AddTaskAsync(TaskDto task)
        {
            user = _userManager.Users.FirstOrDefault()?.Id; // مؤقتا 
            var newtask = new TaskEntity
            {
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority,
                Status = task.Status,
                CreatedById = user
            };
            await _taskRepository.AddAsync(newtask);
            await _taskRepository.SaveAsync();
        }

        public async Task UpdateTaskAsync(TaskDto task)
        {
            var existingTask = await _taskRepository.GetByIdAsync(task.Id);
            if (existingTask != null)
            {
                existingTask.Title = task.Title;
                existingTask.Description = task.Description;
                existingTask.DueDate = task.DueDate;
                existingTask.Priority = task.Priority;
                existingTask.Status = task.Status;

                _taskRepository.Update(existingTask);
                await _taskRepository.SaveAsync();
            }
        }

        public async Task DeleteTaskAsync(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task != null)
            {
                _taskRepository.Delete(task);
                await _taskRepository.SaveAsync();
            }
        }

        public async Task<IEnumerable<TaskEntity>> GetTasksByPriorityAsync(PriorityLevel priority)
        {
            return await _taskRepository.GetTasksByPriorityAsync(priority);
        }
    }
}
