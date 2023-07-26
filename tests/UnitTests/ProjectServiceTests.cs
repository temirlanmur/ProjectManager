using FluentValidation;
using ProjectManager.Application.DTOs.ProjectDTOs;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Interfaces;
using ProjectManager.Application.Services;
using ProjectManager.Application.Validators;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Exceptions;
using UnitTests.Extensions;
using UnitTests.FakeRepositories;

namespace UnitTests
{
    public class ProjectServiceTests
    {
        readonly DataDictionary _dataDictionary;        

        readonly IProjectRepository _fakeProjectRepo;
        readonly IUserRepository _fakeUserRepo;        

        readonly IValidator<CreateProjectDTO> _createProjectDtoValidator;
        readonly IValidator<UpdateProjectDTO> _updateProjectDtoValidator;

        IProjectService SUT;

        public ProjectServiceTests()
        {            
            User privateProjectOwner = new User("PrivateProjectOwner", "Lastname", "email@email.com", "123123Abc").WithId(Guid.NewGuid());
            User publicProjectOwner = new User("PublicProjectOwner", "Lastname", "email@email.com", "123123Abc").WithId(Guid.NewGuid());

            User publicProjectCollaborator = new User("PublicProjectCollaborator", "Lastname", "email@email.com", "123123Abc").WithId(Guid.NewGuid());

            Project privateProject = new Project(privateProjectOwner.Id, "PrivateProject", isPublic: false).WithId(Guid.NewGuid());
            Project publicProject = new Project(publicProjectOwner.Id, "PublicProject", isPublic: true).WithId(Guid.NewGuid());
            {
                publicProject.AddCollaborator(publicProjectCollaborator);
            }                       

            _dataDictionary = new(
                new() { privateProjectOwner, publicProjectOwner, publicProjectCollaborator },
                new() { privateProject, publicProject },
                new() { } );

            _fakeProjectRepo = new FakeProjectRepository(_dataDictionary);
            _fakeUserRepo = new FakeUserRepository(_dataDictionary);

            _createProjectDtoValidator = new CreateProjectDTOValidator();
            _updateProjectDtoValidator = new UpdateProjectDTOValidator();

            SUT = new ProjectService(_fakeProjectRepo, _fakeUserRepo, _createProjectDtoValidator, _updateProjectDtoValidator);
        }

        [Fact]
        public async Task List_DoesNotReturn_PrivateProjects_To_AnonymousUsers()
        {
            // Arrange:
            Project publicProject = _dataDictionary.Projects.First(p => p.Title == "PublicProject");
            Project privateProject = _dataDictionary.Projects.First(p => p.Title == "PrivateProject");

            // Act:
            IEnumerable<Project> result = await SUT.List();

            // Assert:
            Assert.DoesNotContain(privateProject, result);
            Assert.Contains(publicProject, result);                       
        }

        [Fact]
        public async Task List_DoesNotReturn_PrivateProjects_To_RegisteredUsers()
        {
            // Arrange:
            User publicProjectOwner = _dataDictionary.Users.First(u => u.FirstName == "PublicProjectOwner");
            Project publicProject = _dataDictionary.Projects.First(p => p.Title == "PublicProject");
            Project privateProject = _dataDictionary.Projects.First(p => p.Title == "PrivateProject");

            // Act:
            IEnumerable<Project> result = await SUT.List(publicProjectOwner.Id);

            // Assert:
            Assert.DoesNotContain(privateProject, result);
            Assert.Contains(publicProject, result);
        }

        [Fact]
        public async Task List_Returns_PrivateProjects_To_ProjectParticipants()
        {
            // Arrange:
            User privateProjectOwner = _dataDictionary.Users.First(u => u.FirstName == "PrivateProjectOwner");
            Project publicProject = _dataDictionary.Projects.First(p => p.Title == "PublicProject");
            Project privateProject = _dataDictionary.Projects.First(p => p.Title == "PrivateProject");

            // Act:
            IEnumerable<Project> result = await SUT.List(privateProjectOwner.Id);

            // Assert:            
            Assert.Contains(privateProject, result);
            Assert.Contains(publicProject, result);
        }

        [Fact]
        public async Task User_CanCreate_Project()
        {
            // Arrange:
            User user = _dataDictionary.Users.First(u => u.FirstName == "PublicProjectOwner");
            CreateProjectDTO dto = new(user.Id, "New Project");

            // Act:
            Project newProject = await SUT.Create(dto);

            // Assert:
            IEnumerable<Guid> projectIds = (await SUT.List(user.Id)).Select(p => p.Id);
            Assert.Contains(newProject.Id, projectIds);
            Assert.Equal(user.Id, newProject.OwnerId);
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
        public async Task Get_NonexistentProject_Throws_NotFoundException()
        {
            // Assert:
            await Assert.ThrowsAsync<EntityNotFoundException>(() => SUT.Get(null, Guid.NewGuid()));
        }

        [Fact]
        public async Task Get_PrivateProject_Throws_NotFoundException()
        {
            // Arrange:
            User publicProjectOwner = _dataDictionary.Users.First(u => u.FirstName == "PublicProjectOwner");
            Project privateProject = _dataDictionary.Projects.First(p => p.Title == "PrivateProject");

            // Assert:
            await Assert.ThrowsAsync<EntityNotFoundException>(() => SUT.Get(publicProjectOwner.Id, privateProject.Id));
        }

        [Fact]
        public async Task ProjectOwner_CanGet_PrivateProject()
        {
            // Arrange:
            User privateProjectOwner = _dataDictionary.Users.First(u => u.FirstName == "PrivateProjectOwner");
            Project privateProject = _dataDictionary.Projects.First(p => p.Title == "PrivateProject");

            // Act:
            Project retrievedProject = await SUT.Get(privateProjectOwner.Id, privateProject.Id);

            // Assert:
            Assert.Equal(privateProject.Id, retrievedProject.Id);
            Assert.Equal("PrivateProject", retrievedProject.Title);
        }

        [Fact]
        public async Task Owner_Can_UpdateProject()
        {
            // Arrange:
            User projectOwner = _dataDictionary.Users.First(u => u.FirstName == "PublicProjectOwner");
            Project project = _dataDictionary.Projects.First(p => p.Title == "PublicProject");
            UpdateProjectDTO dto = new(projectOwner.Id, project.Id, "NewTitle", "NewDescription", false);            

            // Act:
            var updatedProject = await SUT.Update(dto);

            // Assert:
            Assert.Equal("NewTitle", updatedProject.Title);
            Assert.Equal("NewDescription", updatedProject.Description);
            Assert.False(updatedProject.IsPublic);
        }

        [Fact]
        public async Task NotOwner_Cannot_UpdateProject()
        {
            // Arrange:
            Project project = _dataDictionary.Projects.First(p => p.Title == "PublicProject");
            UpdateProjectDTO dto = new(Guid.NewGuid(), project.Id, "NewTitle", "NewDescription", false);
            
            // Assert:
            await Assert.ThrowsAsync<NotAllowedException>(() => SUT.Update(dto));
        }

        [Fact]
        public async Task Update_PrivateProject_Throws_NotFoundException()
        {
            // Arrange:
            Project project = _dataDictionary.Projects.First(p => p.Title == "PrivateProject");
            UpdateProjectDTO dto = new(Guid.NewGuid(), project.Id, "NewTitle", "NewDescription", false);            

            // Assert:
            await Assert.ThrowsAsync<EntityNotFoundException>(() => SUT.Update(dto));
        }

        [Fact]
        public async Task Owner_Can_DeleteProject()
        {
            // Arrange:
            User projectOwner = _dataDictionary.Users.First(u => u.FirstName == "PublicProjectOwner");
            Project project = _dataDictionary.Projects.First(p => p.Title == "PublicProject");

            // Act:
            await SUT.Delete(projectOwner.Id, project.Id);

            // Assert:
            IEnumerable<Guid> projectIds = (await SUT.List(projectOwner.Id)).Select(p => p.Id);
            Assert.DoesNotContain(project.Id, projectIds);
        }

        [Fact]
        public async Task ProjectOwner_Can_AddCollaborators()
        {
            // Arrange:
            User projectOwner = _dataDictionary.Users.First(u => u.FirstName == "PublicProjectOwner");
            Project project = _dataDictionary.Projects.First(p => p.Title == "PublicProject");
            
            User newCollaborator = _dataDictionary.Users.First(u => u.FirstName == "PrivateProjectOwner");
            
            AddRemoveCollaboratorDTO dto = new(projectOwner.Id, project.Id, newCollaborator.Id);

            // Act:
            await SUT.AddCollaborator(dto);

            // Assert:
            Assert.Contains(project.Collaborators, c => c.Id == newCollaborator.Id);
        }

        [Fact]
        public async Task Adding_AlreadyCollaborator_Throws_AlreadyCollaboratorException()
        {
            // Arrange:
            User projectOwner = _dataDictionary.Users.First(u => u.FirstName == "PublicProjectOwner");
            Project project = _dataDictionary.Projects.First(p => p.Title == "PublicProject");
            
            User collaborator = _dataDictionary.Users.First(u => u.FirstName == "PublicProjectCollaborator");
            
            AddRemoveCollaboratorDTO dto = new(projectOwner.Id, project.Id, collaborator.Id);

            // Assert:
            await Assert.ThrowsAsync<AlreadyCollaboratorException>(() => SUT.AddCollaborator(dto));
        }

        [Fact]
        public async Task ProjectOwner_CanRemove_Collaborator()
        {
            // Arrange:
            User projectOwner = _dataDictionary.Users.First(u => u.FirstName == "PublicProjectOwner");
            Project project = _dataDictionary.Projects.First(p => p.Title == "PublicProject");

            User collaborator = _dataDictionary.Users.First(u => u.FirstName == "PublicProjectCollaborator");

            AddRemoveCollaboratorDTO dto = new(projectOwner.Id, project.Id, collaborator.Id);

            // Act:
            await SUT.RemoveCollaborator(dto);

            // Assert:
            Assert.Empty(project.Collaborators);
        }

        [Fact]
        public async Task Removing_Collaborator_Throws_CollaboratorNotFoundException()
        {
            // Arrange:
            User projectOwner = _dataDictionary.Users.First(u => u.FirstName == "PublicProjectOwner");
            Project project = _dataDictionary.Projects.First(p => p.Title == "PublicProject");

            AddRemoveCollaboratorDTO dto = new(projectOwner.Id, project.Id, Guid.NewGuid());

            // Assert:
            await Assert.ThrowsAsync<CollaboratorNotFoundException>(() => SUT.RemoveCollaborator(dto));
        }
    }
}