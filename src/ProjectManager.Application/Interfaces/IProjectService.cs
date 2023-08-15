using ProjectManager.Application.DTOs.ProjectDTOs;
using ProjectManager.Domain.Entities;

namespace ProjectManager.Application.Interfaces;

public interface IProjectService
{
    /// <summary>
    /// Lists projects available to user.
    /// </summary>
    /// <param name="actorId">Authenticated user's ID, or null, if the user is unauthenticated.</param>
    /// <returns></returns>
    Task<IEnumerable<Project>> ListAsync(Guid? actorId = null);
    
    /// <summary>
    /// Creates new project.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<Project> CreateAsync(CreateProjectDTO dto);

    /// <summary>
    /// Gets project by its ID.
    /// </summary>
    /// <param name="actorId">Authenticated user's ID, or null, if the user is unauthenticated.</param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<Project> GetByIdAsync(Guid? actorId, Guid projectId);

    /// <summary>
    /// Updates project.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<Project> UpdateAsync(UpdateProjectDTO dto);

    /// <summary>
    /// Deletes project.
    /// </summary>
    /// <param name="actorId">ID of the user who makes the request to delete.</param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task DeleteAsync(Guid actorId, Guid projectId);

    /// <summary>
    /// Adds collaborator to the project.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task AddCollaboratorAsync(AddRemoveCollaboratorDTO dto);

    /// <summary>
    /// Removes collaborator from the project.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task RemoveCollaboratorAsync(AddRemoveCollaboratorDTO dto);
}
