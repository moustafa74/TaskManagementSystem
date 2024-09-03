namespace TaskManagementSystem.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public int TaskEntityId { get; set; }
        public TaskEntity TaskEntity { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }

}
