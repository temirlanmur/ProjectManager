using FluentValidation;
using ProjectManager.Application.DTOs.TaskDTOs;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Interfaces;
using ProjectManager.Application.Services;
using ProjectManager.Application.Validators;
using ProjectManager.Domain.Entities;
using UnitTests.Extensions;
using UnitTests.FakeRepositories;

namespace UnitTests;

public class TaskServiceTests
{
    readonly DataDictionary _dataDictionary;

    readonly IProjectRepository _fakeProjectRepo;
    readonly ITaskRepository _fakeTaskRepo;
    readonly ITaskCommentRepository _fakeTaskCommentRepo;    

    readonly ITaskService SUT;

    public TaskServiceTests()
    {
        User projectOwner = new User("ProjectOwner", "Lastname", "email@email.com", "123123Abc").WithId(Guid.NewGuid());
        User collaborator = new User("ProjectCollaborator", "Lastname", "email@email.com", "123123Abc").WithId(Guid.NewGuid());
        User taskAuthor = new User("TaskAuthor", "Lastname", "email@email.com", "123123Abc").WithId(Guid.NewGuid());

        Project project = new Project(projectOwner.Id, "Project", isPublic: true).WithId(Guid.NewGuid());
        {            
            project.AddCollaborator(collaborator);
            project.AddCollaborator(taskAuthor);
        }
        
        ProjectTask task = new ProjectTask(project.Id, taskAuthor.Id, "Task").WithId(Guid.NewGuid());
        TaskComment taskComment = new TaskComment(task.Id, taskAuthor.Id, "Author comment").WithId(Guid.NewGuid());

        _dataDictionary = new(
            new List<User> { projectOwner, collaborator, taskAuthor },
            new List<Project> { project },
            new List<ProjectTask> { task },
            new List<TaskComment> { taskComment });

        _fakeProjectRepo = new FakeProjectRepository(_dataDictionary);
        _fakeTaskRepo = new FakeTaskRepository(_dataDictionary);
        _fakeTaskCommentRepo = new FakeTaskCommentRepository(_dataDictionary);

        IValidator<CreateTaskDTO> _createTaskDtoValidator = new CreateTaskDTOValidator();
        IValidator<UpdateTaskDTO> _updateTaskDtoValidator = new UpdateTaskDTOValidator();
        IValidator<DeleteTaskDTO> _deleteTaskDtoValidator = new DeleteTaskDTOValidator();
        IValidator<AddTaskCommentDTO> _addTaskCommentDtoValidator = new AddTaskCommentDTOValidator();
        IValidator<DeleteTaskCommentDTO> _deleteTaskCommentDtoValidator = new DeleteTaskCommentDTOValidator();

        SUT = new TaskService(
            _fakeProjectRepo,
            _fakeTaskRepo,
            _fakeTaskCommentRepo,
            _createTaskDtoValidator,
            _updateTaskDtoValidator,
            _deleteTaskDtoValidator,
            _addTaskCommentDtoValidator,
            _deleteTaskCommentDtoValidator);
    }

    [Fact]
    public async Task ProjectCollaborator_Can_CreateTask()
    {
        // Arrange:
        Project project = _dataDictionary.Projects.First(p => p.Title == "Project");
        User collaborator = _dataDictionary.Users.First(u => u.FirstName == "ProjectCollaborator");
        CreateTaskDTO dto = new(collaborator.Id, project.Id, "New Task");

        // Act:
        ProjectTask newTask = await SUT.Create(dto);

        // Assert:
        ProjectTask created = _dataDictionary.Tasks.First(t => t.Title == "New Task");
        Assert.Equal(newTask.Id, created.Id);
        Assert.Equal(collaborator.Id, created.AuthorId);
        Assert.Equal(project.Id, created.ProjectId);
        Assert.Equal("New Task", created.Title);
    }

    [Fact]
    public async Task NotCollaborator_Cannot_CreateTask()
    {
        // Arrange:
        Project project = _dataDictionary.Projects.First(p => p.Title == "Project");
        CreateTaskDTO dto = new(Guid.NewGuid(), project.Id, "New Task");

        // Assert:
        await Assert.ThrowsAsync<NotAllowedException>(() => SUT.Create(dto));
    }

    [Fact]
    public async Task ProjectOwner_Can_UpdateAnyTask()
    {
        // Arrange:
        Project project = _dataDictionary.Projects.First(p => p.Title == "Project");
        User projectOwner = _dataDictionary.Users.First(u => u.FirstName == "ProjectOwner");
        ProjectTask task = _dataDictionary.Tasks.First(t => t.Title == "Task");
        UpdateTaskDTO dto = new(projectOwner.Id, project.Id, task.Id, "Updated Task", "Description");

        // Act:
        ProjectTask updatedTask = await SUT.Update(dto);

        // Assert:
        Assert.Equal("Updated Task", updatedTask.Title);
        Assert.Equal("Description", updatedTask.Description);
    }

    [Fact]
    public async Task TaskAuthor_Can_UpdateTheirTasks()
    {
        // Arrange:
        Project project = _dataDictionary.Projects.First(p => p.Title == "Project");
        User taskAuthor = _dataDictionary.Users.First(u => u.FirstName == "TaskAuthor");
        ProjectTask task = _dataDictionary.Tasks.First(t => t.Title == "Task");
        UpdateTaskDTO dto = new(taskAuthor.Id, project.Id, task.Id, "Updated Task", "Description");

        // Act:
        ProjectTask updatedTask = await SUT.Update(dto);

        // Assert:
        Assert.Equal("Updated Task", updatedTask.Title);
        Assert.Equal("Description", updatedTask.Description);
    }

    [Fact]
    public async Task ProjectCollaborator_Cannot_UpdateAnyTask()
    {
        // Arrange:
        Project project = _dataDictionary.Projects.First(p => p.Title == "Project");
        User projectCollaborator = _dataDictionary.Users.First(u => u.FirstName == "ProjectCollaborator");
        ProjectTask task = _dataDictionary.Tasks.First(t => t.Title == "Task");
        UpdateTaskDTO dto = new(projectCollaborator.Id, project.Id, task.Id, "Updated Task", "Description");

        // Assert:
        await Assert.ThrowsAsync<NotAllowedException>(() => SUT.Update(dto));
    }

    [Fact]
    public async Task ProjectOwner_Can_DeleteAnyTask()
    {
        // Arrange:
        Project project = _dataDictionary.Projects.First(p => p.Title == "Project");
        User projectOwner = _dataDictionary.Users.First(u => u.FirstName == "ProjectOwner");
        ProjectTask task = _dataDictionary.Tasks.First(t => t.Title == "Task");
        DeleteTaskDTO dto = new(projectOwner.Id, project.Id, task.Id);

        // Act:
        await SUT.Delete(dto);

        // Assert:
        IEnumerable<Guid> projectTaskIds = _dataDictionary.Tasks.Select(t => t.Id);
        Assert.DoesNotContain(task.Id, projectTaskIds);
    }

    [Fact]
    public async Task TaskAuthor_Can_DeleteTheirTasks()
    {
        // Arrange:
        Project project = _dataDictionary.Projects.First(p => p.Title == "Project");
        User taskAuthor = _dataDictionary.Users.First(u => u.FirstName == "TaskAuthor");
        ProjectTask task = _dataDictionary.Tasks.First(t => t.Title == "Task");
        DeleteTaskDTO dto = new(taskAuthor.Id, project.Id, task.Id);

        // Act:
        await SUT.Delete(dto);

        // Assert:
        IEnumerable<Guid> projectTaskIds = _dataDictionary.Tasks.Select(t => t.Id);
        Assert.DoesNotContain(task.Id, projectTaskIds);
    }

    [Fact]
    public async Task ProjectCollaborator_Can_AddTaskComment()
    {
        // Arrange:
        Project project = _dataDictionary.Projects.First(p => p.Title == "Project");
        User projectCollaborator = _dataDictionary.Users.First(u => u.FirstName == "ProjectCollaborator");
        ProjectTask task = _dataDictionary.Tasks.First(t => t.Title == "Task");
        AddTaskCommentDTO dto = new(projectCollaborator.Id, project.Id, task.Id, "New comment");

        // Act:
        await SUT.AddComment(dto);

        // Assert:
        IEnumerable<TaskComment> taskCommentIds = _dataDictionary.TaskComments.Where(tc => tc.TaskId == task.Id);
        Assert.Contains(taskCommentIds, tc => tc.Text == "New comment");
    }

    [Fact]
    public async Task ProjectCollaborator_Cannot_DeleteAnyTask()
    {
        // Arrange:
        Project project = _dataDictionary.Projects.First(p => p.Title == "Project");
        User projectCollaborator = _dataDictionary.Users.First(u => u.FirstName == "ProjectCollaborator");
        ProjectTask task = _dataDictionary.Tasks.First(t => t.Title == "Task");
        TaskComment comment = _dataDictionary.TaskComments.First(tc => tc.Text == "Author comment");
        DeleteTaskCommentDTO dto = new(projectCollaborator.Id, project.Id, task.Id, comment.Id);

        // Assert:
        await Assert.ThrowsAsync<NotAllowedException>(() => SUT.DeleteComment(dto));
    }
}
