namespace ProjectManager.Application.DTOs.TaskDTOs;

public record AddTaskCommentDTO(
    Guid ActorId,
    Guid ProjectId,
    Guid TaskId,
    string Text);
