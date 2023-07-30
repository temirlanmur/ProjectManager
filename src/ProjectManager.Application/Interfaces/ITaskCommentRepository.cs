using ProjectManager.Domain.Entities;

namespace ProjectManager.Application.Interfaces;

public interface ITaskCommentRepository
{
    Task<TaskComment> Save(TaskComment comment);
    Task Delete(Guid commentId);
}
