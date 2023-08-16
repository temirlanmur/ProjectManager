namespace ProjectManager.WebAPI.Authentication.Service;

public record AuthenticationResult(    
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string Token);

public record LoginDTO(
    string Email,
    string Password);

public record RegisterDTO(
    string Email,
    string Password,
    string FirstName,
    string LastName);
