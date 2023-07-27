using ProjectManager.Domain.Entities;
using System.Reflection;

namespace UnitTests.Extensions;

internal static class ProjectExtensions
{
    const string ConfigurationExceptionMessage = "Project extensions configuration error.";

    internal static Project WithId(this Project project, Guid id)
    {
        var projectIdProp = project
            .GetType()
            .GetProperty("Id")            
            ?? throw new Exception(ConfigurationExceptionMessage);
        projectIdProp.SetValue(project, id);

        return project;
    }    

    internal static Project IncludeTasks(this Project project, List<ProjectTask> tasks)
    {
        var projectTasksField = project
            .GetType()
            .GetField("_tasks", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new Exception(ConfigurationExceptionMessage);
        projectTasksField.SetValue(project, tasks);

        return project;
    }
}
