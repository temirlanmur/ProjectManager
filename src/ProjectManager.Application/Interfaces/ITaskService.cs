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
    Task<ProjectTask> Create(CreateTaskDTO dto);

    /// <summary>
    /// Updates task.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<ProjectTask> Update(UpdateTaskDTO dto);
}
