using ProjectManager.Application.Interfaces;
using ProjectManager.Domain.Entities;

namespace UnitTests.FakeRepositories;

public class FakeProjectRepository : IProjectRepository
{
    readonly List<Project> _projects;

    public FakeProjectRepository(List<Project> projects)
    {
        _projects = projects;
    }

    internal void supplyProject(Project project)
    {
        _projects.Add(project);
    }

    public async Task Delete(Guid projectId)
    {
        _projects.Remove(_projects.First(p => p.Id == projectId));
    }

    public async Task<Project?> GetById(Guid projectId)
    {
        return _projects.FirstOrDefault(p => p.Id == projectId);
    }

    public async Task<Project?> GetByIdWithTasksAndComments(Guid projectId)
    {
        return _projects.FirstOrDefault(p => p.Id == projectId);
    }

    public async Task<IEnumerable<Project>> ListForUser(Guid userId)
    {
        return _projects.Where(p => p.IsPublic == true || p.OwnerId == userId || p.Collaborators.Any(c => c.Id == userId));
    }

    public async Task<IEnumerable<Project>> ListPublic()
    {
        return _projects.Where(p => p.IsPublic);
    }

    public async Task<Project> Save(Project project)
    {
        var existingProject = _projects.FirstOrDefault(p => p.Id == project.Id);

        if (existingProject is not null)
        {
            _projects.Remove(existingProject);
        }

        _projects.Add(project);
        
        return project;
    }
}
