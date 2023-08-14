using System.Net;

namespace ProjectManager.Application.Exceptions;

public class BadRequestException : BaseApplicationException
{
    public BadRequestException(string message = "Bad request.") : base(System.Net.HttpStatusCode.BadRequest, message) { }
}
