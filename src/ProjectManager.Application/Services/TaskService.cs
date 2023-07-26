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

        var project = await _projectRepository.GetById(dto.ProjectId) ?? throw new EntityNotFoundException();

        if (project.OwnerId == dto.ActorId || project.Collaborators.Any(c => c.Id == dto.ActorId))
        {
            ProjectTask task = new(dto.ProjectId, dto.ActorId, dto.Title, dto.Description);
            return await _taskRepository.Save(task);
        }
        else if (project.IsPublic)
        {
            throw new NotAllowedException();
        }
        else
        {
            throw new EntityNotFoundException();
        }
    }

    public async Task<ProjectTask> Update(UpdateTaskDTO dto)
    {
        await _updateTaskDtoValidator.ValidateAndThrowAsync(dto);

        var project = await _projectRepository.GetByIdWithTasks(dto.ProjectId) ?? throw new EntityNotFoundException("Project not found.");

        bool isProjectOwner = project.OwnerId == dto.ActorId;
        bool isCollaborator = project.Collaborators.Any(c => c.Id == dto.ActorId);

        if (!isProjectOwner && !isCollaborator)
        {
            switch (project.IsPublic)
            {
                case true:
                    throw new NotAllowedException();
                case false:
                    throw new EntityNotFoundException("Project not found.");
            }
        }

        var task = project.Tasks.FirstOrDefault(t => t.Id == dto.TaskId) ?? throw new EntityNotFoundException("Task not found.");        

        if (isProjectOwner || task.AuthorId == dto.ActorId)
        {
            task.Title = dto.Title;
            task.Description = dto.Description;

            await _taskRepository.Save(task);
        }
        
        throw new NotAllowedException();        
    }
}
