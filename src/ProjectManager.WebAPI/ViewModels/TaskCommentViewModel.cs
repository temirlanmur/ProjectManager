using ProjectManager.Domain.Entities;

namespace ProjectManager.WebAPI.ViewModels;

public class TaskCommentViewModel
{
    public Guid CommentId { get; set; }
    public Guid TaskId { get; set; }
    public Guid AuthorId { get; set; }
    public string Text { get; set; }

    public static TaskCommentViewModel FromComment(TaskComment comment)
    {
        return new TaskCommentViewModel
        {
            CommentId = comment.Id,
            TaskId = comment.TaskId,
            AuthorId = comment.AuthorId,
            Text = comment.Text
        };
    }
}
