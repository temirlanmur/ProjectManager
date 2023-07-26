namespace ProjectManager.Application.DTOs.TaskDTOs;

public record UpdateTaskDTO(
    Guid ActorId,
    Guid ProjectId,
    Guid TaskId,
    string Title,
    string Description);
