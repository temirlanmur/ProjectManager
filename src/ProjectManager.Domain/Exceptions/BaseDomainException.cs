using System.Net;

namespace ProjectManager.Domain.Exceptions;

public class BaseDomainException : Exception
{
    public BaseDomainException() { }
    public BaseDomainException(string message) : base(message) { }
}
