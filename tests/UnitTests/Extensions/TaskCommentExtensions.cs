using ProjectManager.Domain.Entities;

namespace UnitTests.Extensions;

internal static class TaskCommentExtensions
{
    const string ConfigurationExceptionMessage = "TaskComment extensions configuration error.";

    internal static TaskComment WithId(this TaskComment comment, Guid id)
    {
        var commentIdProp = comment
            .GetType()
            .GetProperty("Id")
            ?? throw new Exception(ConfigurationExceptionMessage);
        commentIdProp.SetValue(comment, id);

        return comment;
    }
}
