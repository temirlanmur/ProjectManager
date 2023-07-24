namespace ProjectManager.Application.DTOs.ProjectDTOs;

public record AddRemoveCollaboratorDTO(
    Guid ActorId,
    Guid ProjectId,
    Guid CollaboratorId);
