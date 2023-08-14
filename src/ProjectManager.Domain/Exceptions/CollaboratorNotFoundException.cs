namespace ProjectManager.Domain.Exceptions;

public class CollaboratorNotFoundException : BaseDomainException
{
    public CollaboratorNotFoundException() : base("Collaborator not found.") { }
}
