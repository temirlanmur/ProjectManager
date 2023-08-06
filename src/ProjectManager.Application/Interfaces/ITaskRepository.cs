using ProjectManager.Domain.Entities;

namespace ProjectManager.Application.Interfaces;

public interface ITaskRepository
{
    Task<ProjectTask> Save(ProjectTask task);
    Task Delete(ProjectTask task);
}
