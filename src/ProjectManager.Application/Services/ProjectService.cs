using FluentValidation;
using ProjectManager.Application.DTOs.ProjectDTOs;
using ProjectManager.Application.Interfaces;
using ProjectManager.Domain.Entities;

namespace ProjectManager.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IValidator<CreateProjectDTO> _createProjectDtoValidator;

    public ProjectService(
        IProjectRepository projectRepository,
        IValidator<CreateProjectDTO> createProjectDtoValidator)
    {
        _projectRepository = projectRepository;
        _createProjectDtoValidator = createProjectDtoValidator;
    }

    public async Task<Project> Create(CreateProjectDTO dto)
    {
        await _createProjectDtoValidator.ValidateAndThrowAsync(dto);

        var project = new Project(dto.ownerId, dto.title, dto.description, dto.isPublic);

        return await _projectRepository.Save(project);
    }

    public async Task<IEnumerable<Project>> ListProjects(Guid? userId = null)
    {
        if (userId is null)
        {
            return await _projectRepository.ListPublic();
        }

        return await _projectRepository.ListForUser(userId.Value);
    }
}
