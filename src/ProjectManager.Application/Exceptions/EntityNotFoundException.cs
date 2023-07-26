using ProjectManager.Domain.Entities;

namespace ProjectManager.Application.Exceptions;

public class EntityNotFoundException : BaseApplicationException
{
    public EntityNotFoundException(string message = "Resource not found.") : base(System.Net.HttpStatusCode.NotFound, message) { }

    public static EntityNotFoundException ForEntity(Type entityType)
    {
        if (entityType == typeof(User))
        {
            return new EntityNotFoundException("User not found.");
        }
        else if (entityType == typeof(Project))
        {
            return new EntityNotFoundException("Project not found.");
        }
        else if (entityType == typeof(ProjectTask))
        {
            return new EntityNotFoundException("Task not found.");
        }
        else if (entityType == typeof(TaskComment))
        {
            return new EntityNotFoundException("Task comment not found.");
        }        

        return new EntityNotFoundException();
    }
}
