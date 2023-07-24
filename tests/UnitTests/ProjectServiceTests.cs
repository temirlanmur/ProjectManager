using FluentValidation;
using ProjectManager.Application.DTOs.ProjectDTOs;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Interfaces;
using ProjectManager.Application.Services;
using ProjectManager.Application.Validators;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Exceptions;
using UnitTests.FakeRepositories;

namespace UnitTests
{
    public class ProjectServiceTests
    {
        readonly Project publicProject;
        readonly Project privateProject;

        readonly User publicProjectOwner;
        readonly User privateProjectOwner;

        IProjectRepository fakeProjectRepo;
        IUserRepository fakeUserRepo;        

        readonly IValidator<CreateProjectDTO> createProjectDtoValidator;
        readonly IValidator<UpdateProjectDTO> updateProjectDtoValidator;

        IProjectService SUT;

        public ProjectServiceTests()
        {
            publicProjectOwner = new User("Pubfirstname", "Lastname", "email@email.com", "123123Abc") { Id = Guid.NewGuid() };
            privateProjectOwner = new User("Privfirstname", "Lastname", "email@email.com", "123123Abc") { Id = Guid.NewGuid() };

            publicProject = new Project(publicProjectOwner.Id, "PublicProject", isPublic: true) { Id = Guid.NewGuid() };
            privateProject = new Project(privateProjectOwner.Id, "PrivateProject", isPublic: false) { Id = Guid.NewGuid() };

            fakeProjectRepo = new FakeProjectRepository(new() { publicProject, privateProject });
            fakeUserRepo = new FakeUserRepository(new() { publicProjectOwner, privateProjectOwner });

            createProjectDtoValidator = new CreateProjectDTOValidator();
            updateProjectDtoValidator = new UpdateProjectDTOValidator();

            SUT = new ProjectService(fakeProjectRepo, fakeUserRepo, createProjectDtoValidator, updateProjectDtoValidator);
        }

        [Fact]
        public async Task AnonymousUsers_DoNotSee_PrivateProjects()
        {            
            // Act:
            IEnumerable<Project> result = await SUT.List();

            // Assert:
            Assert.Contains(publicProject, result);
            Assert.DoesNotContain(privateProject, result);            
        }

        [Fact]
        public async Task RegisteredUsers_CanSee_PrivateProjects()
        {
            // Act:
            IEnumerable<Project> result = await SUT.List(privateProjectOwner.Id);

            // Assert:
            Assert.Contains(privateProject, result);
        }

        [Fact]
        public async Task Create_Throws_ValidationException()
        {
            // Arrange:
            CreateProjectDTO dto = new(Guid.NewGuid(), "A");          

            // Assert:            
            await Assert.ThrowsAsync<ValidationException>(() => SUT.Create(dto));
        }

        [Fact]
        public async Task Getting_NonExistentProject_Throws_NotFoundException()
        {
            // Assert:            
            await Assert.ThrowsAsync<EntityNotFoundException>(() => SUT.Get(Guid.NewGuid()));
        }

        [Fact]
        public async Task Owner_IsAllowedTo_UpdateProject()
        {
            // Arrange:           
            UpdateProjectDTO dto = new(publicProjectOwner.Id, publicProject.Id, "NewTitle", "NewDescription", true);            

            // Act:
            var updatedProject = await SUT.Update(dto);

            // Assert:
            Assert.Equal("NewTitle", updatedProject.Title);
        }

        [Fact]
        public async Task NotOwner_IsNotAllowedTo_UpdateProject()
        {
            // Arrange:            
            UpdateProjectDTO dto = new(Guid.NewGuid(), publicProject.Id, "NewTitle", "NewDescription", true);
            
            // Assert:
            await Assert.ThrowsAsync<NotAllowedException>(() => SUT.Update(dto));
        }

        [Fact]
        public async Task Updating_PrivateProject_Throws_NotFoundException()
        {
            // Arrange:            
            UpdateProjectDTO dto = new(Guid.NewGuid(), privateProject.Id, "NewTitle", "NewDescription", true);            

            // Assert:
            await Assert.ThrowsAsync<EntityNotFoundException>(() => SUT.Update(dto));
        }

        [Fact]
        public async Task NotOwner_IsNotAllowedTo_DeleteProject()
        {
            // Assert:
            await Assert.ThrowsAsync<NotAllowedException>(() => SUT.Delete(Guid.NewGuid(), publicProject.Id));
        }

        [Fact]
        public async Task Deleting_PrivateProject_Throws_NotFoundException()
        {
            // Assert:
            await Assert.ThrowsAsync<EntityNotFoundException>(() => SUT.Delete(Guid.NewGuid(), privateProject.Id));
        }

        [Fact]
        public async Task NotOwner_IsNotAllowedTo_AddCollaborator()
        {
            // Arrange:
            AddRemoveCollaboratorDTO dto = new(Guid.NewGuid(), publicProject.Id, privateProjectOwner.Id);

            // Assert:
            await Assert.ThrowsAsync<NotAllowedException>(() => SUT.AddCollaborator(dto));
        }

        [Fact]
        public async Task Adding_AlreadyCollaborator_Throws_AlreadyCollaboratorException()
        {
            // Arrange:
            User collaborator = new("Collaborator", "Lastname", "email@email.com", "123123Abc") { Id = Guid.NewGuid() };
            ((FakeUserRepository)fakeUserRepo).supplyUser(collaborator);            
            publicProject.AddCollaborator(collaborator);
            AddRemoveCollaboratorDTO dto = new(publicProjectOwner.Id, publicProject.Id, collaborator.Id);

            // Assert:
            await Assert.ThrowsAsync<AlreadyCollaboratorException>(() => SUT.AddCollaborator(dto));
        }
    }
}