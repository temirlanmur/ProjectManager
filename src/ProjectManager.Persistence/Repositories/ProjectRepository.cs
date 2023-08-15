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

    public async Task DeleteAsync(Project project)
    {
        _dbContext.Projects.Remove(project);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Project?> GetByIdAsync(Guid projectId)
    {
        return await _dbContext.Projects
            .FirstOrDefaultAsync(p => p.Id == projectId);
    }

    public async Task<Project?> GetByIdWithTasksAsync(Guid projectId)
    {
        return await _dbContext.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == projectId);
    }

    public async Task<Project?> GetByIdWithTasksAndCommentsAsync(Guid projectId)
    {
        return await _dbContext.Projects
            .Include(p => p.Tasks)
                .ThenInclude(t => t.Comments)
            .FirstOrDefaultAsync(p => p.Id == projectId);
    }

    public async Task<IEnumerable<Project>> ListForUserAsync(Guid userId)
    {
        var projects = await _dbContext.Projects.ToListAsync();
        return projects
            .Where(p => p.IsPublic ||
                        p.OwnerId == userId ||
                        p.Collaborators.Any(c => c.Id == userId));
    }

    public async Task<IEnumerable<Project>> ListPublicAsync()
    {
        return await _dbContext.Projects
            .Where(p => p.IsPublic)
            .ToListAsync();
    }

    public async Task<Project> SaveAsync(Project project)
    {
        _dbContext.Projects.Update(project);
        await _dbContext.SaveChangesAsync();
        return project;
    }
}
