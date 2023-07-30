namespace ProjectManager.Domain.Entities;

public class ProjectTask
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public Guid AuthorId { get; private set; }
    public string Title { get; set; }
    public string Description { get; set; }

    private List<TaskComment> _comments = new();
    public IReadOnlyCollection<TaskComment> Comments => _comments.ToList();

    public ProjectTask(Guid projectId, Guid authorId, string title, string description = "")
    {
        ProjectId = projectId;
        AuthorId = authorId;
        Title = title;
        Description = description;
    }
}
