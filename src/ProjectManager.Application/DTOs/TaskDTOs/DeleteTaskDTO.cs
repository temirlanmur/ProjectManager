namespace ProjectManager.Application.DTOs.TaskDTOs;

public record DeleteTaskDTO(
    Guid ActorId,
    Guid ProjectId,
    Guid TaskId);
