using ProjectManager.Domain.Entities;

namespace UnitTests.FakeRepositories;

public class DataDictionary
{
    readonly List<User> _users = new();
    readonly List<Project> _projects = new();
    readonly List<ProjectTask> _tasks = new();
    readonly List<TaskComment> _taskComments = new();

    public DataDictionary(
        List<User> users,
        List<Project> projects,
        List<ProjectTask> tasks,
        List<TaskComment> taskComments)
    {
        _users = users;
        _projects = projects;
        _tasks = tasks;
        _taskComments = taskComments;
    }

    public List<User> Users => _users;
    public List<Project> Projects => _projects;
    public List<ProjectTask> Tasks => _tasks;
    public List<TaskComment> TaskComments => _taskComments;

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

    public void AddTaskComment(TaskComment comment)
    {
        _taskComments.Add(comment);
    }
}
