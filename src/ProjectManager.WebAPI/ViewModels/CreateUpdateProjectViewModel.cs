namespace ProjectManager.WebAPI.ViewModels;

public record CreateUpdateProjectViewModel(
    string Title,
    string Description,
    bool IsPublic);
