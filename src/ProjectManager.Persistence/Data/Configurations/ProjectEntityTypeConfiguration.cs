using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManager.Domain.Entities;

namespace ProjectManager.Persistence.Data.Configurations;

public class ProjectEntityTypeConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder
            .HasOne(p => p.Owner)
            .WithMany()
            .HasForeignKey(p => p.OwnerId);

        builder
            .HasMany(p => p.Collaborators)
            .WithMany();

        builder
            .HasMany(p => p.Tasks)
            .WithOne()
            .HasForeignKey(t => t.ProjectId);

        builder
            .Navigation(p => p.Collaborators)
            .AutoInclude();
    }
}
