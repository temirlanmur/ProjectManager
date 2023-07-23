using ProjectManager.Domain.Entities;

namespace ProjectManager.Application.Interfaces;

public interface IProjectRepository
{
    Task<IEnumerable<Project>> ListPublic();
    Task<IEnumerable<Project>> ListForUser(Guid userId);
    Task<Project> Save(Project project);
}
