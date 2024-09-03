namespace TaskManagementSystem.Models
{
    public class TeamMember
    {
        public int TeamId { get; set; }
        public Team Team { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public TeamRole Role { get; set; }
    }

    public enum TeamRole
    {
        Member,
        TeamLead
    }
}
