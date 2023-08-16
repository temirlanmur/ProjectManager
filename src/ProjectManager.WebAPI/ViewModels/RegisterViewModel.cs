namespace ProjectManager.WebAPI.ViewModels;

public record RegisterViewModel(
    string Email,
    string Password,
    string FirstName,
    string LastName);
