using ProjectManager.Application.DTOs.ProjectDTOs;
using ProjectManager.Domain.Entities;

namespace ProjectManager.Application.Interfaces;

public interface IProjectService
{
    /// <summary>
    /// Lists projects available to user.
    /// </summary>
    /// <param name="userId">Authenticated user's ID, or null, if the user is unauthenticated.</param>
    /// <returns></returns>
    Task<IEnumerable<Project>> ListProjects(Guid? userId = null);
    
    /// <summary>
    /// Creates new project.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<Project> Create(CreateProjectDTO dto);
}
