using ProjectManager.Domain.Entities;

namespace ProjectManager.Application.Interfaces;

public interface ITaskRepository
{
    Task<ProjectTask> SaveAsync(ProjectTask task);
    Task DeleteAsync(ProjectTask task);
}
