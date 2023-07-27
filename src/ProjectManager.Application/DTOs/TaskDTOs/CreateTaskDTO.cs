namespace ProjectManager.Application.DTOs.TaskDTOs;

public record CreateTaskDTO(
    Guid ActorId,
    Guid ProjectId,
    string Title,
    string Description = "");
