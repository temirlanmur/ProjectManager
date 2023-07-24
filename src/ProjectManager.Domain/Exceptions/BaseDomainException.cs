using System.Net;

namespace ProjectManager.Domain.Exceptions;

public class BaseDomainException : Exception
{
    public int HttpStatusCode { get; }

    public BaseDomainException(HttpStatusCode statusCode, string message) : base(message)
    {
        HttpStatusCode = (int)statusCode;
    }
}
