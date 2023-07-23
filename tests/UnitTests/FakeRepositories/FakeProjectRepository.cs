using ProjectManager.Application.Interfaces;
using ProjectManager.Domain.Entities;

namespace UnitTests.FakeRepositories;

public class FakeProjectRepository : IProjectRepository
{
    private List<Project> _projects;

    public FakeProjectRepository(List<Project> projects)
    {
        _projects = projects;
    }

    public async Task<IEnumerable<Project>> ListForUser(Guid userId)
    {
        return _projects.Where(p => p.IsPublic == true || p.OwnerId == userId);
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
