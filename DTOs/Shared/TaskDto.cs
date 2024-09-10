using TaskManagementSystem.Models;
using TaskStatus = TaskManagementSystem.Models.TaskStatus;

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public PriorityLevel Priority { get; set; }
    public TaskStatus Status { get; set; }
    public string CreatedById { get; set; }
}