using Microsoft.EntityFrameworkCore;
using ProjectManager.Domain.Entities;
using ProjectManager.Persistence.Data.Configurations;

namespace ProjectManager.Persistence.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectTask> Tasks { get; set; }
    public DbSet<TaskComment> TaskComments { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("projectmanager");

        new ProjectEntityTypeConfiguration().Configure(modelBuilder.Entity<Project>());
        new ProjectTaskEntityTypeConfiguration().Configure(modelBuilder.Entity<ProjectTask>());
        new TaskCommentEntityTypeConfiguration().Configure(modelBuilder.Entity<TaskComment>());
    }
}