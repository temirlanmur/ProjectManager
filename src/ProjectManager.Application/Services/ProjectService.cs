using FluentValidation;
using ProjectManager.Application.DTOs.ProjectDTOs;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Interfaces;
using ProjectManager.Domain.Entities;

namespace ProjectManager.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IValidator<CreateProjectDTO> _createProjectDtoValidator;
    private readonly IValidator<UpdateProjectDTO> _updateProjectDtoValidator;

    public ProjectService(
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        IValidator<CreateProjectDTO> createProjectDtoValidator,
        IValidator<UpdateProjectDTO> updateProjectDtoValidator)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _createProjectDtoValidator = createProjectDtoValidator;
        _updateProjectDtoValidator = updateProjectDtoValidator;
    }

    public async Task AddCollaborator(AddRemoveCollaboratorDTO dto)
    {
        var project = await _projectRepository.GetById(dto.ProjectId) ?? throw new EntityNotFoundException("Project not found.");
        var collaborator = await _userRepository.GetById(dto.CollaboratorId) ?? throw new EntityNotFoundException("User not found.");

        if (project.OwnerId != dto.ActorId)
        {
            switch (project.IsPublic || project.Collaborators.Any(c => c.Id == dto.ActorId))
            {
                case true:
                    throw new NotAllowedException();
                case false:
                    throw new EntityNotFoundException();
            }
        }

        project.AddCollaborator(collaborator);
    }

    public async Task<Project> Create(CreateProjectDTO dto)
    {
        await _createProjectDtoValidator.ValidateAndThrowAsync(dto);

        var project = new Project(dto.OwnerId, dto.Title, dto.Description, dto.IsPublic);

        return await _projectRepository.Save(project);
    }

    public async Task Delete(Guid actorId, Guid projectId)
    {
        var project = await _projectRepository.GetById(projectId) ?? throw new EntityNotFoundException();

        if (project.OwnerId != actorId)
        {
            switch (project.IsPublic || project.Collaborators.Any(c => c.Id == actorId))
            {
                case true:
                    throw new NotAllowedException();
                case false:
                    throw new EntityNotFoundException();
            }
        }

        await _projectRepository.Delete(projectId);
    }

    public async Task<Project> Get(Guid? actorId, Guid projectId)
    {
        var project = await _projectRepository.GetByIdWithTasksAndComments(projectId) ?? throw new EntityNotFoundException();

        if (project.IsPublic)
        {
            return project;
        }

        if (actorId is null)
        {
            throw new EntityNotFoundException();
        }

        if (project.OwnerId == actorId || project.Collaborators.Any(c => c.Id == actorId))
        {
            return project;
        }

        throw new EntityNotFoundException();
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
        var project = await _projectRepository.GetById(dto.ProjectId) ?? throw new EntityNotFoundException();

        if (project.OwnerId != dto.ActorId)
        {
            switch (project.IsPublic || project.Collaborators.Any(c => c.Id == dto.ActorId))
            {
                case true:
                    throw new NotAllowedException();
                case false:
                    throw new EntityNotFoundException();
            }
        }

        project.RemoveCollaborator(dto.CollaboratorId);
    }

    public async Task<Project> Update(UpdateProjectDTO dto)
    {
        await _updateProjectDtoValidator.ValidateAndThrowAsync(dto);

        var project = await _projectRepository.GetById(dto.ProjectId) ?? throw new EntityNotFoundException();

        if (project.OwnerId != dto.ActorId)
        {
            switch (project.IsPublic || project.Collaborators.Any(c => c.Id == dto.ActorId))
            {
                case true:
                    throw new NotAllowedException();
                case false:
                    throw new EntityNotFoundException();
            }
        }

        project.Title = dto.Title;
        project.Description = dto.Description;
        project.IsPublic = dto.IsPublic;

        return await _projectRepository.Save(project);
    }
}
