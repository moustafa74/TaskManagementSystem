namespace TaskManagementSystem.Models
{
    public class Attachment
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedAt { get; set; }

        public int TaskEntityId { get; set; }
        public TaskEntity TaskEntity { get; set; }

        public string UploadedById { get; set; }
        public ApplicationUser UploadedBy { get; set; }
    }
}
