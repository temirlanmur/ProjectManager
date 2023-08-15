using ProjectManager.Domain.Entities;

namespace ProjectManager.Application.Interfaces;

public interface ITaskCommentRepository
{
    Task<TaskComment> SaveAsync(TaskComment comment);
    Task DeleteAsync(TaskComment comment);
}
