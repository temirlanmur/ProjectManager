namespace ProjectManager.Application.DTOs.ProjectDTOs;

public record CreateProjectDTO(
    Guid ownerId,
    string title,
    string description = "",
    bool isPublic = false);
