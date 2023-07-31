namespace ProjectManager.Application.DTOs.TaskDTOs;

public record CreateTaskCommentDTO(
    Guid ActorId,
    Guid ProjectId,
    Guid TaskId,
    string Text);
