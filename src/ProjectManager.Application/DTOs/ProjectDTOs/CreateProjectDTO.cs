namespace ProjectManager.Application.DTOs.ProjectDTOs;

public record CreateProjectDTO(
    Guid OwnerId,
    string Title,
    string Description = "",
    bool IsPublic = false);
