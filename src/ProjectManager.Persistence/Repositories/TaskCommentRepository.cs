using ProjectManager.Application.Interfaces;
using ProjectManager.Domain.Entities;
using ProjectManager.Persistence.Data;

namespace ProjectManager.Persistence.Repositories;

public class TaskCommentRepository : ITaskCommentRepository
{
    readonly ApplicationDbContext _dbContext;

    public TaskCommentRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Delete(TaskComment comment)
    {
        _dbContext.TaskComments.Remove(comment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<TaskComment> Save(TaskComment comment)
    {
        _dbContext.TaskComments.Update(comment);
        await _dbContext.SaveChangesAsync();
        return comment;
    }
}
