using FluentValidation;
using ProjectManager.Application.DTOs.ProjectDTOs;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Interfaces;
using ProjectManager.Application.Services;
using ProjectManager.Application.Validators;
using ProjectManager.Domain.Entities;
using UnitTests.FakeRepositories;

namespace UnitTests
{
    public class ProjectServiceTests
    {
        IValidator<CreateProjectDTO> createProjectDtoValidator;
        IValidator<UpdateProjectDTO> updateProjectDtoValidator;

        public ProjectServiceTests()
        {
            createProjectDtoValidator = new CreateProjectDTOValidator();
            updateProjectDtoValidator = new UpdateProjectDTOValidator();
        }

        [Fact]
        public async Task AnonymousUsers_DoNotSee_PrivateProjects()
        {
            // Arrange:
            List<Project> projects = new List<Project>
            {
                new Project(Guid.NewGuid(), "PubProject", isPublic: true),
                new Project(Guid.NewGuid(), "PrivateProject", isPublic: false),
            };
            IProjectRepository fakeRepo = new FakeProjectRepository(projects);
            IProjectService service = new ProjectService(
                fakeRepo,
                createProjectDtoValidator,
                updateProjectDtoValidator);

            // Act:
            IEnumerable<Project> result = await service.List();

            // Assert:
            Assert.DoesNotContain(projects[1], result);
            Assert.Contains(result, p => p.Title == "PubProject");
        }

        [Fact]
        public async Task RegisteredUsers_CanSee_PrivateProjects()
        {
            // Arrange:
            Guid userId = Guid.NewGuid();
            List<Project> projects = new List<Project>
            {
                new Project(userId, "UserProject", isPublic: false),
                new Project(Guid.NewGuid(), "PubProject", isPublic: true),
                new Project(Guid.NewGuid(), "PrivateProject", isPublic: false),
            };
            IProjectRepository fakeRepo = new FakeProjectRepository(projects);
            IProjectService service = new ProjectService(
                fakeRepo,
                createProjectDtoValidator,
                updateProjectDtoValidator);

            // Act:
            IEnumerable<Project> result = await service.List(userId);

            // Assert:
            Assert.Contains(result, p => p.Title == "UserProject");
            Assert.DoesNotContain(projects[2], result);
        }

        [Fact]
        public async Task Create_Throws_ValidationException()
        {
            // Arrange:
            CreateProjectDTO dto = new(Guid.NewGuid(), "A");
            IProjectRepository fakeRepo = new FakeProjectRepository(new List<Project>());
            IProjectService service = new ProjectService(
                fakeRepo,
                createProjectDtoValidator,
                updateProjectDtoValidator);

            // Assert:            
            await Assert.ThrowsAsync<ValidationException>(() => service.Create(dto));
        }

        [Fact]
        public async Task Getting_NonExistentProject_Throws_NotFoundException()
        {
            // Arrange:            
            IProjectRepository fakeRepo = new FakeProjectRepository(new List<Project>());
            IProjectService service = new ProjectService(
                fakeRepo,
                createProjectDtoValidator,
                updateProjectDtoValidator);

            // Assert:            
            await Assert.ThrowsAsync<EntityNotFoundException>(() => service.Get(Guid.NewGuid()));
        }

        [Fact]
        public async Task Owner_IsAllowedTo_UpdateProject()
        {
            // Arrange:
            Guid ownerId = Guid.NewGuid();
            Guid projectId = Guid.NewGuid();
            UpdateProjectDTO dto = new(ownerId, projectId, "NewTitle", "NewDescription", true);
            IProjectRepository fakeRepo = new FakeProjectRepository(new List<Project>
            {
                new Project(ownerId, "OldTitle", isPublic: false) { Id = projectId }
            });
            IProjectService service = new ProjectService(
                fakeRepo,
                createProjectDtoValidator,
                updateProjectDtoValidator);

            // Act:
            var updatedProject = await service.Update(dto);

            // Assert:
            Assert.Equal("NewTitle", updatedProject.Title);
        }

        [Fact]
        public async Task NotOwner_IsNotAllowedTo_UpdateProject()
        {
            // Arrange:
            Guid projectId = Guid.NewGuid();
            UpdateProjectDTO dto = new(Guid.NewGuid(), projectId, "NewTitle", "NewDescription", true);
            IProjectRepository fakeRepo = new FakeProjectRepository(new List<Project>
            {
                new Project(Guid.NewGuid(), "OldTitle", isPublic: true) { Id = projectId }
            });
            IProjectService service = new ProjectService(
                fakeRepo,
                createProjectDtoValidator,
                updateProjectDtoValidator);

            // Assert:
            await Assert.ThrowsAsync<NotAllowedException>(() => service.Update(dto));
        }

        [Fact]
        public async Task Updating_PrivateProject_Throws_NotFoundException()
        {
            // Arrange:
            Guid projectId = Guid.NewGuid();
            UpdateProjectDTO dto = new(Guid.NewGuid(), projectId, "NewTitle", "NewDescription", true);
            IProjectRepository fakeRepo = new FakeProjectRepository(new List<Project>
            {
                new Project(Guid.NewGuid(), "OldTitle", isPublic: false) { Id = projectId }
            });
            IProjectService service = new ProjectService(
                fakeRepo,
                createProjectDtoValidator,
                updateProjectDtoValidator);

            // Assert:
            await Assert.ThrowsAsync<EntityNotFoundException>(() => service.Update(dto));
        }

        [Fact]
        public async Task NotOwner_IsNotAllowedTo_DeleteProject()
        {
            // Arrange:
            Guid collaboratorId = Guid.NewGuid();
            Guid projectId = Guid.NewGuid();
            var collaborator = new User("Firstname", "Lastname", "email@email.com", "123123Abc") { Id = collaboratorId };            
            var project = new Project(Guid.NewGuid(), "Project") { Id = projectId };
            project.AddCollaborator(collaborator);
            IProjectRepository fakeRepo = new FakeProjectRepository(new List<Project> { project });
            IProjectService service = new ProjectService(
                fakeRepo,
                createProjectDtoValidator,
                updateProjectDtoValidator);

            // Assert:
            await Assert.ThrowsAsync<NotAllowedException>(() => service.Delete(collaboratorId, projectId));
        }
    }
}