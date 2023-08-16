using ProjectManager.Domain.Entities;

namespace ProjectManager.WebAPI.ViewModels;

public class TaskViewModel
{
    public Guid TaskId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid ProjectId { get; set; }
    public Guid AuthorId { get; set; }
    public List<TaskCommentViewModel> Comments { get; set; } = new();

    public static TaskViewModel FromTask(ProjectTask task)
    {
        return new TaskViewModel
        {
            TaskId = task.Id,
            Title = task.Title,
            Description = task.Description,
            ProjectId = task.ProjectId,
            AuthorId = task.AuthorId,
            Comments = task.Comments.Select(TaskCommentViewModel.FromComment).ToList(),
        };
    }
}
