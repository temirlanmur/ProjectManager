using FluentValidation;
using ProjectManager.Application.DTOs.ProjectDTOs;
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

        public ProjectServiceTests()
        {
            createProjectDtoValidator = new CreateProjectDTOValidator();
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
            IProjectService service = new ProjectService(fakeRepo, createProjectDtoValidator);

            // Act:
            IEnumerable<Project> result = await service.ListProjects();

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
            IProjectService service = new ProjectService(fakeRepo, createProjectDtoValidator);

            // Act:
            IEnumerable<Project> result = await service.ListProjects(userId);

            // Assert:
            Assert.Contains(result, p => p.Title == "UserProject");
            Assert.DoesNotContain(projects[2], result);
        }

        [Fact]
        public async Task Create_Throws_ValidationException()
        {
            // Arrange:
            CreateProjectDTO dto = new(new Guid(), "A");
            IProjectRepository fakeRepo = new FakeProjectRepository(new List<Project>());
            IProjectService service = new ProjectService(fakeRepo, createProjectDtoValidator);

            // Assert:            
            await Assert.ThrowsAsync<ValidationException>(() => service.Create(dto));
        }
    }
}