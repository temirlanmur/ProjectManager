namespace ProjectManager.Domain.Entities;

public class TaskComment
{
    public Guid Id { get; private set; }
    public Guid TaskId { get; private set; }
    public Guid AuthorId { get; private set; }
    public string Text { get; set; }

    public TaskComment(Guid taskId, Guid authorId, string text)
    {
        TaskId = taskId;
        AuthorId = authorId;
        Text = text;
    }
}
