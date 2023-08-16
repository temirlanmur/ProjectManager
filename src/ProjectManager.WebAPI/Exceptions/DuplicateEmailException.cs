using ProjectManager.Application.Exceptions;
using System.Net;

namespace ProjectManager.WebAPI.Exceptions;

public class DuplicateEmailException : BaseApplicationException
{
    public DuplicateEmailException(string message = "User with the given email already exists.") : base(System.Net.HttpStatusCode.Conflict, message) { }
}
