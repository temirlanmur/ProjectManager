using ProjectManager.Domain.Entities;
using System.Reflection;

namespace UnitTests.Extensions;

internal static class ProjectTaskExtensions
{
    const string ConfigurationExceptionMessage = "ProjectTask extensions configuration error.";

    internal static ProjectTask WithId(this ProjectTask task, Guid id)
    {
        var taskIdProp = task
            .GetType()
            .GetProperty("Id")
            ?? throw new Exception(ConfigurationExceptionMessage);
        taskIdProp.SetValue(task, id);

        return task;
    }

    internal static ProjectTask IncludeComments(this ProjectTask task, List<TaskComment> comments)
    {
        var tasksCommentsField = task
            .GetType()
            .GetField("_comments", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new Exception(ConfigurationExceptionMessage);
        tasksCommentsField.SetValue(task, comments);

        return task;
    }
}
