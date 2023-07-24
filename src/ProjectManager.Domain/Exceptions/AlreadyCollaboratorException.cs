using System.Net;

namespace ProjectManager.Domain.Exceptions;

public class AlreadyCollaboratorException : BaseDomainException
{
    public AlreadyCollaboratorException(string message = "User is already a collaborator.") : base(System.Net.HttpStatusCode.BadRequest, message) { }
}
