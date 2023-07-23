using System.Net;

namespace ProjectManager.Application.Exceptions;

public class BaseApplicationException : Exception
{
    public int HttpStatusCode { get; }

    public BaseApplicationException(HttpStatusCode statusCode, string message) : base(message)
    {
        HttpStatusCode = (int)statusCode;        
    }
}
