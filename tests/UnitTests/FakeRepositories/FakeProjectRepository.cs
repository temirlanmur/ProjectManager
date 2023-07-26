using ProjectManager.Application.Interfaces;
using ProjectManager.Domain.Entities;
using UnitTests.Extensions;

namespace UnitTests.FakeRepositories;

public class FakeProjectRepository : IProjectRepository
{
    readonly DataDictionary _data;

    public FakeProjectRepository(DataDictionary data)
    {
        _data = data;
    }

    public async Task Delete(Guid projectId)
    {       
        _data.Projects.Remove(_data.Projects.First(p => p.Id == projectId));
    }

    public async Task<Project?> GetById(Guid projectId)
    {
        return _data.Projects.FirstOrDefault(p => p.Id == projectId);        
    }

    public async Task<Project?> GetByIdWithTasksAndComments(Guid projectId)
    {
        return _data.Projects.FirstOrDefault(p => p.Id == projectId);
    }

    public async Task<IEnumerable<Project>> ListForUser(Guid userId)
    {
        return _data.Projects.Where(p => p.IsPublic == true || p.OwnerId == userId || p.Collaborators.Any(c => c.Id == userId));
    }

    public async Task<IEnumerable<Project>> ListPublic()
    {
        return _data.Projects.Where(p => p.IsPublic);
    }

    public async Task<Project> Save(Project project)
    {
        var existingProject = _data.Projects.FirstOrDefault(p => p.Id == project.Id);

        if (existingProject is not null)
        {
            _data.Projects.Remove(existingProject);
        }

        _data.Projects.Add(project.WithId(Guid.NewGuid()));
        
        return project;
    }

    public async Task<Project?> GetByIdWithTasks(Guid projectId)
    {
        var project = _data.Projects.FirstOrDefault(p => p.Id == projectId);

        if (project is null)
        {
            return project;
        }

        return project.IncludeTasks(_data.Tasks.Where(t => t.ProjectId == projectId).ToList());
    }
}
