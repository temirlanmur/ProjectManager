using Microsoft.EntityFrameworkCore;
using ProjectManager.Application.Interfaces;
using ProjectManager.Domain.Entities;
using ProjectManager.Persistence.Data;

namespace ProjectManager.Persistence.Repositories;

public class ProjectRepository : IProjectRepository
{
    readonly ApplicationDbContext _dbContext;

    public ProjectRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Delete(Project project)
    {
        _dbContext.Projects.Remove(project);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Project?> GetById(Guid projectId)
    {
        return await _dbContext.Projects
            .FirstOrDefaultAsync(p => p.Id == projectId);
    }

    public async Task<Project?> GetByIdWithTasks(Guid projectId)
    {
        return await _dbContext.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == projectId);
    }

    public async Task<Project?> GetByIdWithTasksAndComments(Guid projectId)
    {
        return await _dbContext.Projects
            .Include(p => p.Tasks)
                .ThenInclude(t => t.Comments)
            .FirstOrDefaultAsync(p => p.Id == projectId);
    }

    public async Task<IEnumerable<Project>> ListForUser(Guid userId)
    {
        var projects = await _dbContext.Projects.ToListAsync();
        return projects
            .Where(p => p.IsPublic ||
                        p.OwnerId == userId ||
                        p.Collaborators.Any(c => c.Id == userId));
    }

    public async Task<IEnumerable<Project>> ListPublic()
    {
        return await _dbContext.Projects
            .Where(p => p.IsPublic)
            .ToListAsync();
    }

    public async Task<Project> Save(Project project)
    {
        _dbContext.Projects.Update(project);
        await _dbContext.SaveChangesAsync();
        return project;
    }
}
