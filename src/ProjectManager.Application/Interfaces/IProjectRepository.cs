using ProjectManager.Domain.Entities;

namespace ProjectManager.Application.Interfaces;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid projectId);
    Task<Project?> GetByIdWithTasksAsync(Guid projectId);
    Task<Project?> GetByIdWithTasksAndCommentsAsync(Guid projectId);
    Task<Project> SaveAsync(Project project);
    Task<IEnumerable<Project>> ListPublicAsync();
    Task<IEnumerable<Project>> ListForUserAsync(Guid userId);
    Task DeleteAsync(Project project);
}
