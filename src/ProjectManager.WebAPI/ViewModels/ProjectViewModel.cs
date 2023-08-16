using ProjectManager.Domain.Entities;

namespace ProjectManager.WebAPI.ViewModels;

public class ProjectViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsPublic { get; set; }
    public Guid OwnerId { get; set; }
    public List<Guid> Collaborators { get; set; } = new();

    public static ProjectViewModel FromProject(Project project)
    {
        return new ProjectViewModel
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            IsPublic = project.IsPublic,
            OwnerId = project.OwnerId,
            Collaborators = project.Collaborators.Select(c => c.Id).ToList(),
        };
    }
}
