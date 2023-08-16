namespace ProjectManager.WebAPI.ViewModels;

public record AuthenticationViewModel(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string Token);
