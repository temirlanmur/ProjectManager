using FluentValidation;
using ProjectManager.Application.DTOs.ProjectDTOs;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Interfaces;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Exceptions;

namespace ProjectManager.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly IValidator<CreateProjectDTO> _createProjectDtoValidator;
    private readonly IValidator<UpdateProjectDTO> _updateProjectDtoValidator;

    public ProjectService(
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        IAuthorizationService authorizationService,
        IValidator<CreateProjectDTO> createProjectDtoValidator,
        IValidator<UpdateProjectDTO> updateProjectDtoValidator)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _authorizationService = authorizationService;
        _createProjectDtoValidator = createProjectDtoValidator;
        _updateProjectDtoValidator = updateProjectDtoValidator;
    }

    public async Task AddCollaboratorAsync(AddRemoveCollaboratorDTO dto)
    {
        var project = await _projectRepository.GetByIdAsync(dto.ProjectId) ?? throw EntityNotFoundException.ForEntity(typeof(Project));

        _authorizationService.ThrowIfNotProjectOwner(dto.ActorId, project);

        var collaborator = await _userRepository.GetByIdAsync(dto.CollaboratorId) ?? throw EntityNotFoundException.ForEntity(typeof(User));
        
        try
        {
            project.AddCollaborator(collaborator);
            await _projectRepository.SaveAsync(project);
        }
        catch (AlreadyCollaboratorException ex)
        {
            throw new BadRequestException(ex.Message);
        }
    }

    public async Task<Project> CreateAsync(CreateProjectDTO dto)
    {
        await _createProjectDtoValidator.ValidateAndThrowAsync(dto);

        var project = new Project(dto.OwnerId, dto.Title, dto.Description, dto.IsPublic);

        return await _projectRepository.SaveAsync(project);
    }

    public async Task DeleteAsync(Guid actorId, Guid projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId) ?? throw EntityNotFoundException.ForEntity(typeof(Project));

        _authorizationService.ThrowIfNotProjectOwner(actorId, project);

        await _projectRepository.DeleteAsync(project);     
    }

    public async Task<Project> GetByIdAsync(Guid? actorId, Guid projectId)
    {
        var project = await _projectRepository.GetByIdWithTasksAndCommentsAsync(projectId) ?? throw EntityNotFoundException.ForEntity(typeof(Project));

        if (project.IsPublic)
        {
            return project;
        }

        if (actorId is null)
        {
            throw EntityNotFoundException.ForEntity(typeof(Project));
        }

        if (project.OwnerId == actorId || project.Collaborators.Any(c => c.Id == actorId))
        {
            return project;
        }

        throw EntityNotFoundException.ForEntity(typeof(Project));
    }

    public async Task<IEnumerable<Project>> ListAsync(Guid? actorId = null)
    {
        if (actorId is null)
        {
            return await _projectRepository.ListPublicAsync();
        }

        return await _projectRepository.ListForUserAsync(actorId.Value);
    }

    public async Task RemoveCollaboratorAsync(AddRemoveCollaboratorDTO dto)
    {
        var project = await _projectRepository.GetByIdAsync(dto.ProjectId) ?? throw EntityNotFoundException.ForEntity(typeof(Project));

        _authorizationService.ThrowIfNotProjectOwner(dto.ActorId, project);

        try
        {
            project.RemoveCollaborator(dto.CollaboratorId);
            await _projectRepository.SaveAsync(project);
        }
        catch (CollaboratorNotFoundException ex)
        {
            throw new BadRequestException(ex.Message);
        }
    }

    public async Task<Project> UpdateAsync(UpdateProjectDTO dto)
    {
        await _updateProjectDtoValidator.ValidateAndThrowAsync(dto);
        
        var project = await _projectRepository.GetByIdAsync(dto.ProjectId) ?? throw EntityNotFoundException.ForEntity(typeof(Project));

        _authorizationService.ThrowIfNotProjectOwner(dto.ActorId, project);

        project.Title = dto.Title;
        project.Description = dto.Description;
        project.IsPublic = dto.IsPublic;
        return await _projectRepository.SaveAsync(project);
    }
}
