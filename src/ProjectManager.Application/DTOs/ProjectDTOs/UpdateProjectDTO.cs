namespace ProjectManager.Application.DTOs.ProjectDTOs;

public record UpdateProjectDTO(
    Guid ActorId,
    Guid ProjectId,    
    string Title,
    string Description,
    bool IsPublic);
