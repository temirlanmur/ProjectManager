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

    readonly IValidator<CreateTaskDTO> _createTaskDtoValidator;
    readonly IValidator<UpdateTaskDTO> _updateTaskDtoValidator;

    ITaskService SUT;

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

        _dataDictionary = new(
            new List<User> { projectOwner, collaborator, taskAuthor },
            new List<Project> { project },
            new List<ProjectTask> { task });

        _fakeProjectRepo = new FakeProjectRepository(_dataDictionary);
        _fakeTaskRepo = new FakeTaskRepository(_dataDictionary);

        _createTaskDtoValidator = new CreateTaskDTOValidator();
        _updateTaskDtoValidator = new UpdateTaskDTOValidator();

        SUT = new TaskService(
            _fakeProjectRepo,
            _fakeTaskRepo,
            _createTaskDtoValidator,
            _updateTaskDtoValidator);
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
        var project = _dataDictionary.Projects.First(p => p.Title == "Project");
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
        User taskAuthor = _dataDictionary.Users.First(u => u.FirstName == "ProjectCollaborator");
        ProjectTask task = _dataDictionary.Tasks.First(t => t.Title == "Task");
        UpdateTaskDTO dto = new(taskAuthor.Id, project.Id, task.Id, "Updated Task", "Description");

        // Assert:
        await Assert.ThrowsAsync<NotAllowedException>(() => SUT.Update(dto));
    }
}
