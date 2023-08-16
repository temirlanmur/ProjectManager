using ProjectManager.Application.Exceptions;

namespace ProjectManager.WebAPI.Exceptions;

public class InvalidCredentialsException : BaseApplicationException
{
    public InvalidCredentialsException(string message = "Either user email, or password is incorrect.") : base(System.Net.HttpStatusCode.BadRequest, message) { }
}
