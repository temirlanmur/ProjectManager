using ProjectManager.Domain.Entities;

namespace ProjectManager.WebAPI.ViewModels;

public class ProjectDetailedViewModel : ProjectViewModel
{
    public List<TaskViewModel> Tasks { get; set; } = new();

    public new static ProjectDetailedViewModel FromProject(Project project)
    {
        return new ProjectDetailedViewModel
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            IsPublic = project.IsPublic,
            OwnerId = project.OwnerId,
            Collaborators = project.Collaborators.Select(c => c.Id).ToList(),
            Tasks = project.Tasks.Select(TaskViewModel.FromTask).ToList(),
        };
    }
}
