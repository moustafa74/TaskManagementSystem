namespace TaskManagementSystem.Models
{
    public class TaskAssignment
    {
        public int TaskEntityId { get; set; }
        public TaskEntity TaskEntity { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int? TeamId { get; set; }
        public Team Team { get; set; }
    }
}
