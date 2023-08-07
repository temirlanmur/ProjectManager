using FluentValidation;
using ProjectManager.Application.DTOs.TaskDTOs;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Interfaces;
using ProjectManager.Application.Services;
using ProjectManager.Application.Validators;
using ProjectManager.Domain.Entities;
using ProjectManager.Persistence.Repositories;

namespace IntegrationTests
{
    public class TaskServiceIntegrationTests : IClassFixture<TestDatabaseFixture>
    {
        public TestDatabaseFixture Fixture { get; }

        IAuthorizationService AuthorizationService => new AuthorizationService();
        IValidator<CreateTaskDTO> CreateTaskDtoValidator => new CreateTaskDTOValidator();
        IValidator<UpdateTaskDTO> UpdateTaskDtoValidator => new UpdateTaskDTOValidator();
        IValidator<DeleteTaskDTO> DeleteTaskDtoValidator => new DeleteTaskDTOValidator();
        IValidator<CreateTaskCommentDTO> CreateTaskCommentDtoValidator => new CreateTaskCommentDTOValidator();
        IValidator<DeleteTaskCommentDTO> DeleteTaskCommentDtoValidator => new DeleteTaskCommentDTOValidator();

        public TaskServiceIntegrationTests(TestDatabaseFixture fixture)
        {
            Fixture = fixture;            
        }

        [Fact]
        public async Task ProjectCollaborator_Can_AddTaskComment()
        {
            // Arrange:
            using var context = Fixture.CreateContext();

            Project project = context.Projects.First(p => p.Title == "Project");
            ProjectTask task = context.Tasks.First(t => t.Title == "Task");
            User collaborator = context.Users.First(u => u.FirstName == "ProjectCollaborator");

            CreateTaskCommentDTO dto = new(collaborator.Id, project.Id, task.Id, "New comment");

            IProjectRepository projectRepository = new ProjectRepository(context);
            ITaskRepository taskRepository = new TaskRepository(context);
            ITaskCommentRepository taskCommentRepository = new TaskCommentRepository(context);

            ITaskService SUT = new TaskService(
                projectRepository,
                taskRepository,
                taskCommentRepository,
                AuthorizationService,
                CreateTaskDtoValidator,
                UpdateTaskDtoValidator,
                DeleteTaskDtoValidator,
                CreateTaskCommentDtoValidator,
                DeleteTaskCommentDtoValidator);

            // Act:
            await SUT.AddComment(dto);

            // Assert:
            IEnumerable<TaskComment> taskCommentIds = context.TaskComments.Where(tc => tc.TaskId == task.Id);
            Assert.Contains(taskCommentIds, tc => tc.Text == "New comment");
        }

        [Fact]
        public async Task ProjectOwner_Can_DeleteAnyTaskComment()
        {
            // Arrange:
            using var context = Fixture.CreateContext();

            Project project = context.Projects.First(p => p.Title == "Project");
            ProjectTask task = context.Tasks.First(t => t.Title == "Task");
            TaskComment taskComment = context.TaskComments.First(tc => tc.Text == "Comment 2");
            User projectOwner = context.Users.First(u => u.FirstName == "ProjectOwner");

            DeleteTaskCommentDTO dto = new(projectOwner.Id, project.Id, task.Id, taskComment.Id);

            IProjectRepository projectRepository = new ProjectRepository(context);
            ITaskRepository taskRepository = new TaskRepository(context);
            ITaskCommentRepository taskCommentRepository = new TaskCommentRepository(context);

            ITaskService SUT = new TaskService(
                projectRepository,
                taskRepository,
                taskCommentRepository,
                AuthorizationService,
                CreateTaskDtoValidator,
                UpdateTaskDtoValidator,
                DeleteTaskDtoValidator,
                CreateTaskCommentDtoValidator,
                DeleteTaskCommentDtoValidator);

            // Act:
            await SUT.DeleteComment(dto);

            // Assert:
            IEnumerable<Guid> taskCommentIds = context.TaskComments.Select(tc => tc.Id);
            Assert.DoesNotContain(taskComment.Id, taskCommentIds);
        }

        [Fact]
        public async Task NonCollaborator_Cannot_AddTaskComment()
        {
            // Arrange:
            using var context = Fixture.CreateContext();

            Project project = context.Projects.First(p => p.Title == "Project");
            ProjectTask task = context.Tasks.First(t => t.Title == "Task");
            User randomUser = context.Users.First(u => u.FirstName == "RandomUser");

            CreateTaskCommentDTO dto = new(randomUser.Id, project.Id, task.Id, "New comment");

            IProjectRepository projectRepository = new ProjectRepository(context);
            ITaskRepository taskRepository = new TaskRepository(context);
            ITaskCommentRepository taskCommentRepository = new TaskCommentRepository(context);

            ITaskService SUT = new TaskService(
                projectRepository,
                taskRepository,
                taskCommentRepository,
                AuthorizationService,
                CreateTaskDtoValidator,
                UpdateTaskDtoValidator,
                DeleteTaskDtoValidator,
                CreateTaskCommentDtoValidator,
                DeleteTaskCommentDtoValidator);

            // Assert:
            await Assert.ThrowsAsync<EntityNotFoundException>(() => SUT.AddComment(dto));
        }
    }
}