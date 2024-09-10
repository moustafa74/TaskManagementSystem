using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Api.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for the entities
        public DbSet<TaskEntity> TaskEntities { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<TaskAssignment> TaskAssignments { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<TaskDependency> TaskDependencies { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configuration for TeamMember (Composite Key)
            builder.Entity<TeamMember>()
                .HasKey(tm => new { tm.TeamId, tm.UserId });

            builder.Entity<TeamMember>()
                .HasOne(tm => tm.Team)
                .WithMany(t => t.TeamMembers)
                .HasForeignKey(tm => tm.TeamId);

            builder.Entity<TeamMember>()
                .HasOne(tm => tm.User)
                .WithMany(u => u.TeamMembers)
                .HasForeignKey(tm => tm.UserId);

            // Configuration for TaskAssignment (Composite Key)
            builder.Entity<TaskAssignment>()
                .HasKey(ta => new { ta.TaskEntityId, ta.UserId });

            // Configure relationship between TaskAssignment and TaskEntity
            builder.Entity<TaskAssignment>()
                .HasOne(ta => ta.TaskEntity)
                .WithMany(t => t.TaskAssignments)
                .HasForeignKey(ta => ta.TaskEntityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship between TaskAssignment and ApplicationUser
            builder.Entity<TaskAssignment>()
                .HasOne(ta => ta.User)
                .WithMany(u => u.TaskAssignments)
                .HasForeignKey(ta => ta.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship between TaskAssignment and Team
            builder.Entity<TaskAssignment>()
            .HasOne(ta => ta.Team)
            .WithMany(t => t.TaskAssignments)
            .HasForeignKey(ta => ta.TeamId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

            // Configuration for TaskDependency (Composite Key)
            builder.Entity<TaskDependency>()
                .HasKey(td => new { td.TaskEntityId, td.DependentTaskEntityId });

            builder.Entity<TaskDependency>()
                .HasOne(td => td.TaskEntity)
                .WithMany(t => t.TaskDependencies)
                .HasForeignKey(td => td.TaskEntityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TaskDependency>()
                .HasOne(td => td.DependentTaskEntity)
                .WithMany()
                .HasForeignKey(td => td.DependentTaskEntityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Attachment>()
                .HasOne(a => a.TaskEntity)
                .WithMany(t => t.Attachments)
                .HasForeignKey(a => a.TaskEntityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Comment>()
                .HasOne(c => c.TaskEntity)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TaskEntityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TaskEntity>()
                .HasOne(t => t.CreatedBy)
                .WithMany(u => u.TasksCreated)
                .HasForeignKey(t => t.CreatedById);

        }
    }
}
