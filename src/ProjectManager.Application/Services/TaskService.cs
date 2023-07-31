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
    private readonly IValidator<CreateTaskDTO> _createTaskDtoValidator;
    private readonly IValidator<UpdateTaskDTO> _updateTaskDtoValidator;
    private readonly IValidator<DeleteTaskDTO> _deleteTaskDtoValidator;
    private readonly IValidator<CreateTaskCommentDTO> _createTaskCommentDtoValidator;
    private readonly IValidator<DeleteTaskCommentDTO> _deleteTaskCommentDtoValidator;

    public TaskService(
        IProjectRepository projectRepository,
        ITaskRepository taskRepository,
        ITaskCommentRepository taskCommentRepository,
        IValidator<CreateTaskDTO> createTaskDtoValidator,
        IValidator<UpdateTaskDTO> updateTaskDtoValidator,
        IValidator<DeleteTaskDTO> deleteTaskDtoValidator,
        IValidator<CreateTaskCommentDTO> createTaskCommentDtoValidator,
        IValidator<DeleteTaskCommentDTO> deleteTaskCommentDtoValidator)
    {
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
        _taskCommentRepository = taskCommentRepository;
        _createTaskDtoValidator = createTaskDtoValidator;
        _updateTaskDtoValidator = updateTaskDtoValidator;
        _deleteTaskDtoValidator = deleteTaskDtoValidator;
        _createTaskCommentDtoValidator = createTaskCommentDtoValidator;
        _deleteTaskCommentDtoValidator = deleteTaskCommentDtoValidator;
    }

    public async Task<TaskComment> AddComment(CreateTaskCommentDTO dto)
    {
        await _createTaskCommentDtoValidator.ValidateAndThrowAsync(dto);

        var project = await _projectRepository.GetByIdWithTasks(dto.ProjectId) ?? throw EntityNotFoundException.ForEntity(typeof(Project));

        AuthorizeProjectOwnerOrCollaboratorRequirement(dto.ActorId, project);

        bool isProjectOwner = project.OwnerId == dto.ActorId;
        bool isCollaborator = project.Collaborators.Any(c => c.Id == dto.ActorId);

        var task = project.Tasks.FirstOrDefault(t => t.Id == dto.TaskId) ?? throw EntityNotFoundException.ForEntity(typeof(ProjectTask));

        TaskComment comment = new(dto.TaskId, dto.ActorId, dto.Text);
        return await _taskCommentRepository.Save(comment);
    }

    public async Task<ProjectTask> Create(CreateTaskDTO dto)
    {
        await _createTaskDtoValidator.ValidateAndThrowAsync(dto);

        var project = await _projectRepository.GetById(dto.ProjectId) ?? throw EntityNotFoundException.ForEntity(typeof(Project));

        AuthorizeProjectOwnerOrCollaboratorRequirement(dto.ActorId, project);

        ProjectTask task = new(dto.ProjectId, dto.ActorId, dto.Title, dto.Description);
        return await _taskRepository.Save(task);
    }

    public async Task Delete(DeleteTaskDTO dto)
    {
        await _deleteTaskDtoValidator.ValidateAndThrowAsync(dto);

        var project = await _projectRepository.GetByIdWithTasks(dto.ProjectId) ?? throw EntityNotFoundException.ForEntity(typeof(Project));

        AuthorizeProjectOwnerOrCollaboratorRequirement(dto.ActorId, project);

        bool isProjectOwner = project.OwnerId == dto.ActorId;
        bool isCollaborator = project.Collaborators.Any(c => c.Id == dto.ActorId);

        var task = project.Tasks.FirstOrDefault(t => t.Id == dto.TaskId) ?? throw EntityNotFoundException.ForEntity(typeof(ProjectTask));

        if (isProjectOwner || (isCollaborator && task.AuthorId == dto.ActorId))
        {
            await _taskRepository.Delete(task.Id);
            return;
        }

        throw new NotAllowedException();
    }

    public async Task DeleteComment(DeleteTaskCommentDTO dto)
    {
        await _deleteTaskCommentDtoValidator.ValidateAndThrowAsync(dto);

        var project = await _projectRepository.GetByIdWithTasksAndComments(dto.ProjectId) ?? throw EntityNotFoundException.ForEntity(typeof(Project));

        AuthorizeProjectOwnerOrCollaboratorRequirement(dto.ActorId, project);

        bool isProjectOwner = project.OwnerId == dto.ActorId;
        bool isCollaborator = project.Collaborators.Any(c => c.Id == dto.ActorId);

        var task = project.Tasks.FirstOrDefault(t => t.Id == dto.TaskId) ?? throw EntityNotFoundException.ForEntity(typeof(ProjectTask));
        var comment = task.Comments.FirstOrDefault(c => c.Id == dto.TaskCommentId) ?? throw EntityNotFoundException.ForEntity(typeof(TaskComment));

        if (isProjectOwner || (isCollaborator && comment.AuthorId == dto.ActorId))
        {
            await _taskCommentRepository.Delete(task.Id);
            return;
        }

        throw new NotAllowedException();
    }

    public async Task<ProjectTask> Update(UpdateTaskDTO dto)
    {
        await _updateTaskDtoValidator.ValidateAndThrowAsync(dto);

        var project = await _projectRepository.GetByIdWithTasks(dto.ProjectId) ?? throw EntityNotFoundException.ForEntity(typeof(Project));

        AuthorizeProjectOwnerOrCollaboratorRequirement(dto.ActorId, project);

        bool isProjectOwner = project.OwnerId == dto.ActorId;
        bool isCollaborator = project.Collaborators.Any(c => c.Id == dto.ActorId);

        var task = project.Tasks.FirstOrDefault(t => t.Id == dto.TaskId) ?? throw EntityNotFoundException.ForEntity(typeof(ProjectTask));

        if (isProjectOwner || (isCollaborator && task.AuthorId == dto.ActorId))
        {
            task.Title = dto.Title;
            task.Description = dto.Description;

            return await _taskRepository.Save(task);
        }

        throw new NotAllowedException();
    }

    /// <summary>
    /// Checks if the given actor is project owner or collaborator.
    /// If so, simply returns.
    /// Otherwise throws suitable exception.
    /// </summary>
    /// <param name="actorId"></param>
    /// <param name="project"></param>
    /// <exception cref="NotAllowedException"></exception>
    /// <exception cref="EntityNotFoundException"></exception>
    public void AuthorizeProjectOwnerOrCollaboratorRequirement(Guid actorId, Project project)
    {
        if (project.OwnerId == actorId || project.Collaborators.Any(c => c.Id == actorId))
        {
            return;
        }

        if (project.IsPublic)
        {
            throw new NotAllowedException();
        }

        throw EntityNotFoundException.ForEntity(typeof(Project));
    }
}
