using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.DAL.Repositories;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.BLL
{
    public class TaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITeamRepository _teamRepository;
        private readonly IGenericRepository<TaskAssignment> _taskAssignmentRepository;

        public TaskService(ITaskRepository taskRepository, UserManager<ApplicationUser> userRepository, ITeamRepository teamRepository, IGenericRepository<TaskAssignment> taskAssignmentRepository)
        {
            _taskRepository = taskRepository;
            _userManager = userRepository;
            _teamRepository = teamRepository;
            _taskAssignmentRepository = taskAssignmentRepository;   
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

        //    var isTeamLead = task.TaskAssignments.Any(ta => ta.Team.TeamLeadId == teamLeadId);
        //    return isTeamLead;
        //}

        public async Task<IEnumerable<TaskDto>> GetTasksByUserIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
            {
                var tasks = await GetAllTasksAsync();
                return tasks.Select(MapToTaskDto);
            }
            else if (roles.Contains("TeamLead"))
            {
                var teamId = await _teamRepository.GetTeamIdByUserIdAsync(userId);
                if (teamId.HasValue)
                {
                    var tasks = await _taskRepository.GetTasksByTeamIdAsync(teamId.Value);
                    return tasks.Select(MapToTaskDto);
                }
                else
                {
                    return Enumerable.Empty<TaskDto>();
                }
            }
            else //Regular User
            {
                var tasks = await _taskRepository.GetTasksByUserIdAsync(userId);
                return tasks.Select(MapToTaskDto);
            }
        }

        // Convert Task Entity to TaskDto
        private TaskDto MapToTaskDto(TaskEntity task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority,
                Status = task.Status,
            };
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
            try
            {
                var newtask = new TaskEntity
                {
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    Priority = task.Priority,
                    Status = task.Status,
                    CreatedById = task.CreatedById,
                };
                await _taskRepository.AddAsync(newtask);
                await _taskRepository.SaveAsync();
                var taskAssignment = new TaskAssignment
                {
                    TaskEntityId = newtask.Id,
                    UserId = newtask.CreatedById,
                    TeamId = null
                };
                await _taskRepository.AddTaskAssignmentAsync(taskAssignment);
                await _taskRepository.SaveAsync();
            }
            catch (Exception)
            {

                throw;
            }
            
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

                await _taskRepository.UpdateAsync(existingTask);
                await _taskRepository.SaveAsync();
            }
        }

        public async Task DeleteTaskAsync(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task != null)
            {
                try
                {
                    var taskAssignments = await _taskAssignmentRepository.GetAllAsync(ta => ta.TaskEntityId == task.Id);
                    if (taskAssignments != null && taskAssignments.Any())
                    {
                        _taskAssignmentRepository.DeleteRange(taskAssignments);
                    }

                    await _taskRepository.DeleteAsync(task);
                    await _taskRepository.SaveAsync();
                }
                catch (Exception)
                {

                    throw;
                }          
            }
        }

        public async Task<IEnumerable<TaskEntity>> GetTasksByPriorityAsync(PriorityLevel priority)
        {
            return await _taskRepository.GetTasksByPriorityAsync(priority);
        }
    }
}
