namespace ProjectManager.Domain.Exceptions;

public class CollaboratorNotFoundException : BaseDomainException
{
    public CollaboratorNotFoundException(string message = "Collaborator not found.") : base(System.Net.HttpStatusCode.BadRequest, message) { }
}
