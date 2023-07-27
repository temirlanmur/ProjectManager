using FluentValidation;
using ProjectManager.Application.DTOs.TaskDTOs;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Interfaces;
using ProjectManager.Domain.Entities;
using System.Xml;

namespace ProjectManager.Application.Services;

public class TaskService : ITaskService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IValidator<CreateTaskDTO> _createTaskDtoValidator;
    private readonly IValidator<UpdateTaskDTO> _updateTaskDtoValidator;

    public TaskService(
        IProjectRepository projectRepository,
        ITaskRepository taskRepository,
        IValidator<CreateTaskDTO> createTaskDtoValidator,
        IValidator<UpdateTaskDTO> updateTaskDtoValidator)
    {
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
        _createTaskDtoValidator = createTaskDtoValidator;
        _updateTaskDtoValidator = updateTaskDtoValidator;
    }

    public async Task<ProjectTask> Create(CreateTaskDTO dto)
    {
        await _createTaskDtoValidator.ValidateAndThrowAsync(dto);

        var project = await _projectRepository.GetById(dto.ProjectId) ?? throw EntityNotFoundException.ForEntity(typeof(Project));

        if (project.OwnerId == dto.ActorId || project.Collaborators.Any(c => c.Id == dto.ActorId))
        {
            ProjectTask task = new(dto.ProjectId, dto.ActorId, dto.Title, dto.Description);
            return await _taskRepository.Save(task);
        }
        
        if (project.IsPublic)
        {
            throw new NotAllowedException();
        }

        throw EntityNotFoundException.ForEntity(typeof(Project));
    }

    public async Task<ProjectTask> Update(UpdateTaskDTO dto)
    {
        await _updateTaskDtoValidator.ValidateAndThrowAsync(dto);

        var project = await _projectRepository.GetByIdWithTasks(dto.ProjectId) ?? throw EntityNotFoundException.ForEntity(typeof(Project));        

        bool isProjectOwner = project.OwnerId == dto.ActorId;
        bool isCollaborator = project.Collaborators.Any(c => c.Id == dto.ActorId);

        if (isProjectOwner || isCollaborator)
        {
            var task = project.Tasks.FirstOrDefault(t => t.Id == dto.TaskId) ?? throw EntityNotFoundException.ForEntity(typeof(ProjectTask));

            if (isProjectOwner || (isCollaborator && task.AuthorId == dto.ActorId))
            {
                task.Title = dto.Title;
                task.Description = dto.Description;

                return await _taskRepository.Save(task);
            }

            throw new NotAllowedException();
        }

        if (project.IsPublic)
        {
            throw new NotAllowedException();
        }

        throw EntityNotFoundException.ForEntity(typeof(Project));
    }
}
