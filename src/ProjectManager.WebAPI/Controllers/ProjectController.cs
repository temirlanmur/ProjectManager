using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Application.DTOs.ProjectDTOs;
using ProjectManager.Application.Interfaces;
using ProjectManager.Domain.Entities;
using ProjectManager.WebAPI.ViewModels;

namespace ProjectManager.WebAPI.Controllers;

[Route("projects")]
public class ProjectController : BaseApiController
{
    private readonly IProjectService _projectService;

    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    /// <summary>
    /// Lists all projects available to user.
    /// </summary>
    /// <returns></returns>
    /// <response code="200">List of projects.</response>
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectViewModel>), 200)]
    public async Task<IActionResult> List()
    {
        IEnumerable<Project> projects;

        if (User.Identity?.IsAuthenticated ?? false)
        {
            projects = await _projectService.ListAsync(AuthenticatedUserId);
        }
        else
        {
            projects = await _projectService.ListAsync();
        }

        IEnumerable<ProjectViewModel> model = projects.Select(ProjectViewModel.FromProject);

        return Ok(model);
    }

    /// <summary>
    /// Creates new project.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <response code="201">Newly created project information.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectDetailedViewModel), 201)]
    public async Task<IActionResult> Create([FromBody] CreateUpdateProjectViewModel request)
    {
        Project project = await _projectService.CreateAsync(new CreateProjectDTO(
            AuthenticatedUserId,
            request.Title,
            request.Description,
            request.IsPublic));
        ProjectDetailedViewModel model = ProjectDetailedViewModel.FromProject(project);

        return CreatedAtRoute(model.Id, model);
    }

    /// <summary>
    /// Retrieves project by ID.
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns></returns>
    /// <response code="200">Project information.</response>
    [AllowAnonymous]
    [HttpGet("{projectId}")]
    [ProducesResponseType(typeof(ProjectDetailedViewModel), 200)]
    public async Task<IActionResult> GetById([FromRoute] Guid projectId)
    {
        Project project;

        if (User.Identity?.IsAuthenticated ?? false)
        {
            project = await _projectService.GetByIdAsync(AuthenticatedUserId, projectId);
        }
        else
        {
            project = await _projectService.GetByIdAsync(null, projectId);
        }

        ProjectDetailedViewModel model = ProjectDetailedViewModel.FromProject(project);

        return Ok(model);
    }

    /// <summary>
    /// Updates project.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <response code="200">Updated project information.</response>
    [HttpPatch("{projectId}")]
    [ProducesResponseType(typeof(ProjectViewModel), 200)]
    public async Task<IActionResult> Update([FromRoute] Guid projectId, [FromBody] CreateUpdateProjectViewModel request)
    {
        Project project = await _projectService.UpdateAsync(new UpdateProjectDTO(
            AuthenticatedUserId,
            projectId,
            request.Title,
            request.Description,
            request.IsPublic));
        ProjectViewModel model = ProjectViewModel.FromProject(project);

        return Ok(model);
    }

    /// <summary>
    /// Deletes project.
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns></returns>
    /// <response code="200">Indicates successful deletion.</response>
    [HttpDelete("{projectId}")]
    public async Task<IActionResult> Delete([FromRoute] Guid projectId)
    {
        await _projectService.DeleteAsync(AuthenticatedUserId, projectId);

        return Ok();
    }

    /// <summary>
    /// Adds collaborator to the project.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <response code="200">Indicates successful operation.</response>
    [HttpPost("{projectId}/collaborators")]
    public async Task<IActionResult> AddCollaborator([FromRoute] Guid projectId, [FromBody] AddCollaboratorViewModel request)
    {
        await _projectService.AddCollaboratorAsync(new AddRemoveCollaboratorDTO(
            AuthenticatedUserId,
            projectId,
            request.CollaboratorId));

        return Ok();
    }

    /// <summary>
    /// Removes collaborator from the project.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="collaboratorId"></param>
    /// <returns></returns>
    /// <response code="200">Indicates successful operation.</response>
    [HttpDelete("{projectId}/collaborators/{collaboratorId}")]
    public async Task<IActionResult> RemoveCollaborator([FromRoute] Guid projectId, [FromRoute] Guid collaboratorId)
    {
        await _projectService.RemoveCollaboratorAsync(new AddRemoveCollaboratorDTO(
            AuthenticatedUserId,
            projectId,
            collaboratorId));

        return Ok();
    }
}
