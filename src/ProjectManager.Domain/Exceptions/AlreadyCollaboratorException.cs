using System.Net;

namespace ProjectManager.Domain.Exceptions;

public class AlreadyCollaboratorException : BaseDomainException
{
    public AlreadyCollaboratorException() : base("User is already a collaborator.") { }
}
