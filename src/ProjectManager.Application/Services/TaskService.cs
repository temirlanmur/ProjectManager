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
    private readonly IValidator<AddTaskCommentDTO> _addTaskCommentDtoValidator;
    private readonly IValidator<DeleteTaskCommentDTO> _deleteTaskCommentDtoValidator;

    public TaskService(
        IProjectRepository projectRepository,
        ITaskRepository taskRepository,
        ITaskCommentRepository taskCommentRepository,
        IValidator<CreateTaskDTO> createTaskDtoValidator,
        IValidator<UpdateTaskDTO> updateTaskDtoValidator,
        IValidator<DeleteTaskDTO> deleteTaskDtoValidator,
        IValidator<AddTaskCommentDTO> addTaskCommentDtoValidator,
        IValidator<DeleteTaskCommentDTO> deleteTaskCommentDtoValidator)
    {
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
        _taskCommentRepository = taskCommentRepository;
        _createTaskDtoValidator = createTaskDtoValidator;
        _updateTaskDtoValidator = updateTaskDtoValidator;
        _deleteTaskDtoValidator = deleteTaskDtoValidator;
        _addTaskCommentDtoValidator = addTaskCommentDtoValidator;
        _deleteTaskCommentDtoValidator = deleteTaskCommentDtoValidator;
    }

    public async Task<TaskComment> AddComment(AddTaskCommentDTO dto)
    {
        await _addTaskCommentDtoValidator.ValidateAndThrowAsync(dto);

        var project = await _projectRepository.GetByIdWithTasks(dto.ProjectId) ?? throw EntityNotFoundException.ForEntity(typeof(Project));

        bool isProjectOwner = project.OwnerId == dto.ActorId;
        bool isCollaborator = project.Collaborators.Any(c => c.Id == dto.ActorId);

        if (isProjectOwner || isCollaborator)
        {
            var task = project.Tasks.FirstOrDefault(t => t.Id == dto.TaskId) ?? throw EntityNotFoundException.ForEntity(typeof(ProjectTask));

            TaskComment comment = new(dto.TaskId, dto.ActorId, dto.Text);
            return await _taskCommentRepository.Save(comment);
        }

        if (project.IsPublic)
        {
            throw new NotAllowedException();
        }

        throw EntityNotFoundException.ForEntity(typeof(Project));
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

    public async Task Delete(DeleteTaskDTO dto)
    {
        await _deleteTaskDtoValidator.ValidateAndThrowAsync(dto);

        var project = await _projectRepository.GetByIdWithTasks(dto.ProjectId) ?? throw EntityNotFoundException.ForEntity(typeof(Project));

        bool isProjectOwner = project.OwnerId == dto.ActorId;
        bool isCollaborator = project.Collaborators.Any(c => c.Id == dto.ActorId);

        if (isProjectOwner || isCollaborator)
        {
            var task = project.Tasks.FirstOrDefault(t => t.Id == dto.TaskId) ?? throw EntityNotFoundException.ForEntity(typeof(ProjectTask));

            if (isProjectOwner || (isCollaborator && task.AuthorId == dto.ActorId))
            {
                await _taskRepository.Delete(task.Id);
                return;
            }

            throw new NotAllowedException();
        }

        if (project.IsPublic)
        {
            throw new NotAllowedException();
        }

        throw EntityNotFoundException.ForEntity(typeof(Project));
    }

    public async Task DeleteComment(DeleteTaskCommentDTO dto)
    {
        await _deleteTaskCommentDtoValidator.ValidateAndThrowAsync(dto);

        var project = await _projectRepository.GetByIdWithTasksAndComments(dto.ProjectId) ?? throw EntityNotFoundException.ForEntity(typeof(Project));

        bool isProjectOwner = project.OwnerId == dto.ActorId;
        bool isCollaborator = project.Collaborators.Any(c => c.Id == dto.ActorId);

        if (isProjectOwner || isCollaborator)
        {
            var task = project.Tasks.FirstOrDefault(t => t.Id == dto.TaskId) ?? throw EntityNotFoundException.ForEntity(typeof(ProjectTask));
            var comment = task.Comments.FirstOrDefault(c => c.Id == dto.TaskCommentId) ?? throw EntityNotFoundException.ForEntity(typeof(TaskComment));

            if (isProjectOwner || (isCollaborator && comment.AuthorId == dto.ActorId))
            {
                await _taskCommentRepository.Delete(task.Id);
                return;
            }

            throw new NotAllowedException();
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
