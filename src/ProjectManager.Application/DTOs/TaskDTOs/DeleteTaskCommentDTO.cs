namespace ProjectManager.Application.DTOs.TaskDTOs;

public record DeleteTaskCommentDTO(
    Guid ActorId,
    Guid ProjectId,
    Guid TaskId,
    Guid TaskCommentId);
