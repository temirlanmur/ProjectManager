using ProjectManager.Application.DTOs.TaskDTOs;
using ProjectManager.Domain.Entities;

namespace ProjectManager.Application.Interfaces;

public interface ITaskService
{
    /// <summary>
    /// Creates new task for the project.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<ProjectTask> CreateAsync(CreateTaskDTO dto);

    /// <summary>
    /// Updates task.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<ProjectTask> UpdateAsync(UpdateTaskDTO dto);

    /// <summary>
    /// Deletes task.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task DeleteAsync(DeleteTaskDTO dto);

    /// <summary>
    /// Adds task comment.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<TaskComment> AddCommentAsync(CreateTaskCommentDTO dto);

    /// <summary>
    /// Deletes task comment.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task DeleteCommentAsync(DeleteTaskCommentDTO dto);
}
