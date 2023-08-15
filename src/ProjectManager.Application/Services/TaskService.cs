using FluentValidation;
using ProjectManager.Application.DTOs.TaskDTOs;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Interfaces;
using ProjectManager.Domain.Entities;

namespace ProjectManager.Application.Services;

public class TaskService : ITaskService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly ITaskCommentRepository _taskCommentRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly IValidator<CreateTaskDTO> _createTaskDtoValidator;
    private readonly IValidator<UpdateTaskDTO> _updateTaskDtoValidator;
    private readonly IValidator<DeleteTaskDTO> _deleteTaskDtoValidator;
    private readonly IValidator<CreateTaskCommentDTO> _createTaskCommentDtoValidator;
    private readonly IValidator<DeleteTaskCommentDTO> _deleteTaskCommentDtoValidator;

    public TaskService(
        IProjectRepository projectRepository,
        ITaskRepository taskRepository,
        ITaskCommentRepository taskCommentRepository,
        IAuthorizationService authorizationService,
        IValidator<CreateTaskDTO> createTaskDtoValidator,
        IValidator<UpdateTaskDTO> updateTaskDtoValidator,
        IValidator<DeleteTaskDTO> deleteTaskDtoValidator,
        IValidator<CreateTaskCommentDTO> createTaskCommentDtoValidator,
        IValidator<DeleteTaskCommentDTO> deleteTaskCommentDtoValidator)
    {
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
        _taskCommentRepository = taskCommentRepository;
        _authorizationService = authorizationService;
        _createTaskDtoValidator = createTaskDtoValidator;
        _updateTaskDtoValidator = updateTaskDtoValidator;
        _deleteTaskDtoValidator = deleteTaskDtoValidator;
        _createTaskCommentDtoValidator = createTaskCommentDtoValidator;
        _deleteTaskCommentDtoValidator = deleteTaskCommentDtoValidator;
    }

    public async Task<TaskComment> AddCommentAsync(CreateTaskCommentDTO dto)
    {
        await _createTaskCommentDtoValidator.ValidateAndThrowAsync(dto);

        var project = await _projectRepository.GetByIdWithTasksAsync(dto.ProjectId) ?? throw EntityNotFoundException.ForEntity(typeof(Project));

        _authorizationService.ThrowIfNotProjectOwnerOrCollaborator(dto.ActorId, project);

        bool isProjectOwner = project.OwnerId == dto.ActorId;
        bool isCollaborator = project.Collaborators.Any(c => c.Id == dto.ActorId);

        var task = project.Tasks.FirstOrDefault(t => t.Id == dto.TaskId) ?? throw EntityNotFoundException.ForEntity(typeof(ProjectTask));

        TaskComment comment = new(dto.TaskId, dto.ActorId, dto.Text);
        return await _taskCommentRepository.SaveAsync(comment);
    }

    public async Task<ProjectTask> CreateAsync(CreateTaskDTO dto)
    {
        await _createTaskDtoValidator.ValidateAndThrowAsync(dto);

        var project = await _projectRepository.GetByIdAsync(dto.ProjectId) ?? throw EntityNotFoundException.ForEntity(typeof(Project));

        _authorizationService.ThrowIfNotProjectOwnerOrCollaborator(dto.ActorId, project);

        ProjectTask task = new(dto.ProjectId, dto.ActorId, dto.Title, dto.Description);
        return await _taskRepository.SaveAsync(task);
    }

    public async Task DeleteAsync(DeleteTaskDTO dto)
    {
        await _deleteTaskDtoValidator.ValidateAndThrowAsync(dto);

        var project = await _projectRepository.GetByIdWithTasksAsync(dto.ProjectId) ?? throw EntityNotFoundException.ForEntity(typeof(Project));

        _authorizationService.ThrowIfNotProjectOwnerOrCollaborator(dto.ActorId, project);

        bool isProjectOwner = project.OwnerId == dto.ActorId;
        bool isCollaborator = project.Collaborators.Any(c => c.Id == dto.ActorId);

        var task = project.Tasks.FirstOrDefault(t => t.Id == dto.TaskId) ?? throw EntityNotFoundException.ForEntity(typeof(ProjectTask));

        if (isProjectOwner || (isCollaborator && task.AuthorId == dto.ActorId))
        {
            await _taskRepository.DeleteAsync(task);
            return;
        }

        throw new NotAllowedException();
    }

    public async Task DeleteCommentAsync(DeleteTaskCommentDTO dto)
    {
        await _deleteTaskCommentDtoValidator.ValidateAndThrowAsync(dto);

        var project = await _projectRepository.GetByIdWithTasksAndCommentsAsync(dto.ProjectId) ?? throw EntityNotFoundException.ForEntity(typeof(Project));

        _authorizationService.ThrowIfNotProjectOwnerOrCollaborator(dto.ActorId, project);

        bool isProjectOwner = project.OwnerId == dto.ActorId;
        bool isCollaborator = project.Collaborators.Any(c => c.Id == dto.ActorId);

        var task = project.Tasks.FirstOrDefault(t => t.Id == dto.TaskId) ?? throw EntityNotFoundException.ForEntity(typeof(ProjectTask));
        var comment = task.Comments.FirstOrDefault(c => c.Id == dto.TaskCommentId) ?? throw EntityNotFoundException.ForEntity(typeof(TaskComment));

        if (isProjectOwner || (isCollaborator && comment.AuthorId == dto.ActorId))
        {
            await _taskCommentRepository.DeleteAsync(comment);
            return;
        }

        throw new NotAllowedException();
    }

    public async Task<ProjectTask> UpdateAsync(UpdateTaskDTO dto)
    {
        await _updateTaskDtoValidator.ValidateAndThrowAsync(dto);

        var project = await _projectRepository.GetByIdWithTasksAsync(dto.ProjectId) ?? throw EntityNotFoundException.ForEntity(typeof(Project));

        _authorizationService.ThrowIfNotProjectOwnerOrCollaborator(dto.ActorId, project);

        bool isProjectOwner = project.OwnerId == dto.ActorId;
        bool isCollaborator = project.Collaborators.Any(c => c.Id == dto.ActorId);

        var task = project.Tasks.FirstOrDefault(t => t.Id == dto.TaskId) ?? throw EntityNotFoundException.ForEntity(typeof(ProjectTask));

        if (isProjectOwner || (isCollaborator && task.AuthorId == dto.ActorId))
        {
            task.Title = dto.Title;
            task.Description = dto.Description;

            return await _taskRepository.SaveAsync(task);
        }

        throw new NotAllowedException();
    }
}
