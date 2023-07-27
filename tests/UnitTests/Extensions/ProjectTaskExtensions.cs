using ProjectManager.Domain.Entities;

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
}
