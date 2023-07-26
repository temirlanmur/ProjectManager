using ProjectManager.Domain.Entities;
using System.Reflection;

namespace UnitTests.Extensions;

internal static class UserExtensions
{
    const string ConfigurationExceptionMessage = "User extensions configuration error.";

    internal static User WithId(this User user, Guid id)
    {
        var userIdProp = user
            .GetType()
            .GetProperty("Id")
            ?? throw new Exception(ConfigurationExceptionMessage);
        userIdProp.SetValue(user, id);

        return user;
    }
}
