using ProjectManager.Domain.Entities;

namespace UnitTests.FakeRepositories;

public class DataDictionary
{
    readonly List<User> _users = new();
    readonly List<Project> _projects = new();
    readonly List<ProjectTask> _tasks = new();    

    public DataDictionary(
        List<User> users,
        List<Project> projects,
        List<ProjectTask> tasks)
    {
        _users = users;
        _projects = projects;
        _tasks = tasks;
    }

    public List<User> Users => _users;
    public List<Project> Projects => _projects;
    public List<ProjectTask> Tasks => _tasks;

    public void AddUser(User user)
    {
        _users.Add(user);
    }

    public void AddProject(Project project)
    {
        _projects.Add(project);
    }

    public void AddTask(ProjectTask task)
    {
        _tasks.Add(task);
    }
}
