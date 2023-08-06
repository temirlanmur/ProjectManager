using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManager.Domain.Entities;

namespace ProjectManager.Persistence.Data.Configurations;

public class ProjectTaskEntityTypeConfiguration : IEntityTypeConfiguration<ProjectTask>
{
    public void Configure(EntityTypeBuilder<ProjectTask> builder)
    {
        builder
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(t => t.AuthorId);

        builder
            .HasMany(t => t.Comments)
            .WithOne()
            .HasForeignKey(tc => tc.TaskId);
    }
}
