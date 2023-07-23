namespace ProjectManager.Application.Exceptions;

public class EntityNotFoundException : BaseApplicationException
{
    public EntityNotFoundException(string message = "Resource not found.") : base(System.Net.HttpStatusCode.NotFound, message) { }
}
