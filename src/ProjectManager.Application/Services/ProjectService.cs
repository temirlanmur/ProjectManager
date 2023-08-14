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

    public async Task AddCollaborator(AddRemoveCollaboratorDTO dto)
    {
        var project = await _projectRepository.GetById(dto.ProjectId) ?? throw EntityNotFoundException.ForEntity(typeof(Project));

        _authorizationService.ThrowIfNotProjectOwner(dto.ActorId, project);

        var collaborator = await _userRepository.GetById(dto.CollaboratorId) ?? throw EntityNotFoundException.ForEntity(typeof(User));
        
        try
        {
            project.AddCollaborator(collaborator);
        }
        catch (AlreadyCollaboratorException ex)
        {
            throw new BadRequestException(ex.Message);
        }
    }

    public async Task<Project> Create(CreateProjectDTO dto)
    {
        await _createProjectDtoValidator.ValidateAndThrowAsync(dto);

        var project = new Project(dto.OwnerId, dto.Title, dto.Description, dto.IsPublic);

        return await _projectRepository.Save(project);
    }

    public async Task Delete(Guid actorId, Guid projectId)
    {
        var project = await _projectRepository.GetById(projectId) ?? throw EntityNotFoundException.ForEntity(typeof(Project));

        _authorizationService.ThrowIfNotProjectOwner(actorId, project);

        await _projectRepository.Delete(project);     
    }

    public async Task<Project> Get(Guid? actorId, Guid projectId)
    {
        var project = await _projectRepository.GetByIdWithTasksAndComments(projectId) ?? throw EntityNotFoundException.ForEntity(typeof(Project));

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

    public async Task<IEnumerable<Project>> List(Guid? actorId = null)
    {
        if (actorId is null)
        {
            return await _projectRepository.ListPublic();
        }

        return await _projectRepository.ListForUser(actorId.Value);
    }

    public async Task RemoveCollaborator(AddRemoveCollaboratorDTO dto)
    {
        var project = await _projectRepository.GetById(dto.ProjectId) ?? throw EntityNotFoundException.ForEntity(typeof(Project));

        _authorizationService.ThrowIfNotProjectOwner(dto.ActorId, project);

        try
        {
            project.RemoveCollaborator(dto.CollaboratorId);
        }
        catch (CollaboratorNotFoundException ex)
        {
            throw new BadRequestException(ex.Message);
        }
    }

    public async Task<Project> Update(UpdateProjectDTO dto)
    {
        await _updateProjectDtoValidator.ValidateAndThrowAsync(dto);
        
        var project = await _projectRepository.GetById(dto.ProjectId) ?? throw EntityNotFoundException.ForEntity(typeof(Project));

        _authorizationService.ThrowIfNotProjectOwner(dto.ActorId, project);

        project.Title = dto.Title;
        project.Description = dto.Description;
        project.IsPublic = dto.IsPublic;
        return await _projectRepository.Save(project);
    }
}
