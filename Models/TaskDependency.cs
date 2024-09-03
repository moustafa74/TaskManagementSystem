namespace TaskManagementSystem.Models
{
    public class TaskDependency
    {
        public int TaskEntityId { get; set; }
        public TaskEntity TaskEntity { get; set; }

        public int DependentTaskEntityId { get; set; }
        public TaskEntity DependentTaskEntity { get; set; }
    }
}
