using ProjectManager.Domain.Entities;

namespace ProjectManager.Application.Interfaces;

public interface IProjectRepository
{
    Task<Project?> GetById(Guid projectId);
    Task<Project?> GetByIdWithTasksAndComments(Guid projectId);
    Task<Project> Save(Project project);
    Task<IEnumerable<Project>> ListPublic();
    Task<IEnumerable<Project>> ListForUser(Guid userId);
}
