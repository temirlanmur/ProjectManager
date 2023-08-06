using ProjectManager.Application.Interfaces;
using ProjectManager.Domain.Entities;
using ProjectManager.Persistence.Data;

namespace ProjectManager.Persistence.Repositories;

public class TaskRepository : ITaskRepository
{
    readonly ApplicationDbContext _dbContext;

    public TaskRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Delete(ProjectTask task)
    {
        _dbContext.Tasks.Remove(task);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<ProjectTask> Save(ProjectTask task)
    {
        _dbContext.Tasks.Update(task);
        await _dbContext.SaveChangesAsync();
        return task;
    }
}
