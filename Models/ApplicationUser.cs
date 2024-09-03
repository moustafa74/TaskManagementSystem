using Microsoft.AspNetCore.Identity;

namespace TaskManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public ICollection<TaskAssignment> TaskAssignments { get; set; }
        public ICollection<TeamMember> TeamMembers { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<TaskEntity> TasksCreated { get; set; }
    }
}