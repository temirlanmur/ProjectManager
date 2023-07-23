namespace ProjectManager.Domain.Entities;

public class ProjectTask
{
    public Guid Id { get; init; }
    public Guid AuthorId { get; private set; }
    public string Title { get; set; }
    public string Description { get; set; }    

    public IReadOnlyCollection<TaskComment> Comments { get; private set; } = new List<TaskComment>();

    public ProjectTask(Guid authorId, string title, string description = "")
    {
        AuthorId = authorId;
        Title = title;
        Description = description;
    }
}
