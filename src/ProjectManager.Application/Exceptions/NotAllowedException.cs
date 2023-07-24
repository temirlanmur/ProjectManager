namespace ProjectManager.Application.Exceptions;

public class NotAllowedException : BaseApplicationException
{
    public NotAllowedException(string message = "Forbidden.") : base(System.Net.HttpStatusCode.Forbidden, message) { }
}
