using FluentValidation;
using ProjectManager.Application.DTOs.ProjectDTOs;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Interfaces;
using ProjectManager.Domain.Entities;

namespace ProjectManager.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IValidator<CreateProjectDTO> _createProjectDtoValidator;
    private readonly IValidator<UpdateProjectDTO> _updateProjectDtoValidator;

    public ProjectService(
        IProjectRepository projectRepository,
        IValidator<CreateProjectDTO> createProjectDtoValidator,
        IValidator<UpdateProjectDTO> updateProjectDtoValidator)
    {
        _projectRepository = projectRepository;
        _createProjectDtoValidator = createProjectDtoValidator;
        _updateProjectDtoValidator = updateProjectDtoValidator;
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

    public async Task<Project> Get(Guid projectId)
    {
        var project = await _projectRepository.GetByIdWithTasksAndComments(projectId) ?? throw new EntityNotFoundException();

        return project;
    }

    public async Task<IEnumerable<Project>> List(Guid? userId = null)
    {
        if (userId is null)
        {
            return await _projectRepository.ListPublic();
        }

        return await _projectRepository.ListForUser(userId.Value);
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
