using Microsoft.AspNetCore.Mvc;
using ProjectManager.Application.DTOs.TaskDTOs;
using ProjectManager.Application.Interfaces;
using ProjectManager.Domain.Entities;
using ProjectManager.WebAPI.ViewModels;

namespace ProjectManager.WebAPI.Controllers;

[Route("projects/{projectId}/tasks")]
public class TaskController : BaseApiController
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    /// <summary>
    /// Creates new project task.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <response code="200">Newly created task information.</response>
    [HttpPost]
    [ProducesResponseType(typeof(TaskViewModel), 200)]
    public async Task<IActionResult> Create([FromRoute] Guid projectId,[FromBody] CreateUpdateTaskViewModel request)
    {
        ProjectTask task = await _taskService.CreateAsync(new CreateTaskDTO(
            AuthenticatedUserId,
            projectId,
            request.Title,
            request.Description));
        TaskViewModel model = TaskViewModel.FromTask(task);

        return Ok(model);
    }

    /// <summary>
    /// Updates task.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="taskId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <response code="200">Updated task information.</response>
    [HttpPatch("{taskId}")]
    [ProducesResponseType(typeof(TaskViewModel), 200)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid projectId,
        [FromRoute] Guid taskId,
        [FromBody] CreateUpdateTaskViewModel request)
    {
        ProjectTask task = await _taskService.UpdateAsync(new UpdateTaskDTO(
            AuthenticatedUserId,
            projectId,
            taskId,
            request.Title,
            request.Description));
        TaskViewModel model = TaskViewModel.FromTask(task);

        return Ok(model);
    }

    /// <summary>
    /// Deletes task.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="taskId"></param>
    /// <returns></returns>
    /// <response code="200">Indicates successful deletion.</response>
    [HttpDelete("{taskId}")]
    public async Task<IActionResult> Delete([FromRoute] Guid projectId, [FromRoute] Guid taskId)
    {
        await _taskService.DeleteAsync(new DeleteTaskDTO(
            AuthenticatedUserId,
            projectId,
            taskId));        

        return Ok();
    }

    /// <summary>
    /// Adds comment to the task.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="taskId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <response code="200">Newly created comment information.</response>
    [HttpPost("{taskId}/comments")]
    [ProducesResponseType(typeof(TaskCommentViewModel), 200)]
    public async Task<IActionResult> AddComent(
        [FromRoute] Guid projectId,
        [FromRoute] Guid taskId,
        [FromBody] CreateTaskCommentViewModel request)
    {
        TaskComment comment = await _taskService.AddCommentAsync(new CreateTaskCommentDTO(
            AuthenticatedUserId,
            projectId,
            taskId,
            request.Text));
        TaskCommentViewModel model = TaskCommentViewModel.FromComment(comment);

        return Ok(model);
    }

    /// <summary>
    /// Deletes task comment.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="taskId"></param>
    /// <param name="commentId"></param>
    /// <returns></returns>
    /// <response code="200">Indicates successful deletion.</response>
    [HttpDelete("{taskId}/comments/{commentId}")]
    public async Task<IActionResult> DeleteComment(
        [FromRoute] Guid projectId,
        [FromRoute] Guid taskId,
        [FromRoute] Guid commentId)
    {
        await _taskService.DeleteCommentAsync(new DeleteTaskCommentDTO(
            AuthenticatedUserId,
            projectId,
            taskId,
            commentId));

        return Ok();
    }
}
