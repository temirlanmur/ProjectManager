using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Interfaces;
using ProjectManager.Application.Services;
using ProjectManager.Domain.Entities;
using UnitTests.Extensions;
using UnitTests.FakeRepositories;

namespace UnitTests;

public class AuthorizationServiceTests
{
    readonly DataDictionary _dataDictionary;
    readonly IAuthorizationService SUT;

    public AuthorizationServiceTests()
    {
        User projectOwner = new User("ProjectOwner", "Lastname", "email@email.com", "123123Abc").WithId(Guid.NewGuid());
        User projectCollaborator = new User("ProjectCollaborator", "Lastname", "email@email.com", "123123Abc").WithId(Guid.NewGuid());
        
        Project publicProject = new Project(projectOwner.Id, "PublicProject", isPublic: true).WithId(Guid.NewGuid());
        Project privateProject = new Project(projectOwner.Id, "PrivateProject", isPublic: false).WithId(Guid.NewGuid());

        publicProject.AddCollaborator(projectCollaborator);
        privateProject.AddCollaborator(projectCollaborator);        

        _dataDictionary = new(
                new List<User> { projectOwner, projectCollaborator },
                new List<Project> { privateProject, publicProject },
                new List<ProjectTask> { },
                new List<TaskComment> { });

        SUT = new AuthorizationService();
    }

    [Fact]
    public void ProjectOwner_Satisfies_ProjectOwnerRequirement()
    {
        // Arrange:
        Project project = _dataDictionary.Projects.First(p => p.Title == "PrivateProject");
        User projectOwner = _dataDictionary.Users.First(u => u.FirstName == "ProjectOwner");

        // Act:
        Exception? exception = Record.Exception(() => SUT.AuthorizeProjectOwnerRequirement(projectOwner.Id, project));

        // Assert:
        Assert.Null(exception);
    }

    [Fact]
    public void ProjectCollaborator_DoesNotSatisfy_ProjectOwnerRequirement_Throws_NotAllowed()
    {
        // Arrange:
        Project project = _dataDictionary.Projects.First(p => p.Title == "PrivateProject");
        User projectCollaborator = _dataDictionary.Users.First(u => u.FirstName == "ProjectCollaborator");

        // Assert:
        Assert.Throws<NotAllowedException>(() => SUT.AuthorizeProjectOwnerRequirement(projectCollaborator.Id, project));
    }

    [Fact]
    public void NonParticipant_DoesNotSatisfy_ProjectOwnerRequirement_Throws_NotAllowed_ForPublicProject()
    {
        // Arrange:
        Project project = _dataDictionary.Projects.First(p => p.Title == "PublicProject");        

        // Assert:
        Assert.Throws<NotAllowedException>(() => SUT.AuthorizeProjectOwnerRequirement(Guid.NewGuid(), project));
    }

    [Fact]
    public void NonParticipant_DoesNotSatisfy_ProjectOwnerRequirement_Throws_NotFound_ForPrivateProject()
    {
        // Arrange:
        Project project = _dataDictionary.Projects.First(p => p.Title == "PrivateProject");

        // Assert:
        Assert.Throws<EntityNotFoundException>(() => SUT.AuthorizeProjectOwnerRequirement(Guid.NewGuid(), project));
    }

    [Fact]
    public void ProjectOwner_Satisfies_ProjectOwnerOrCollaboratorRequirement()
    {
        // Arrange:
        Project project = _dataDictionary.Projects.First(p => p.Title == "PrivateProject");
        User projectOwner = _dataDictionary.Users.First(u => u.FirstName == "ProjectOwner");

        // Act:
        Exception? exception = Record.Exception(() => SUT.AuthorizeProjectOwnerOrCollaboratorRequirement(projectOwner.Id, project));

        // Assert:
        Assert.Null(exception);
    }

    [Fact]
    public void ProjectCollaborator_Satisfies_ProjectOwnerOrCollaboratorRequirement()
    {
        // Arrange:
        Project project = _dataDictionary.Projects.First(p => p.Title == "PrivateProject");
        User projectCollaborator = _dataDictionary.Users.First(u => u.FirstName == "ProjectCollaborator");

        // Act:
        Exception? exception = Record.Exception(() => SUT.AuthorizeProjectOwnerOrCollaboratorRequirement(projectCollaborator.Id, project));

        // Assert:
        Assert.Null(exception);
    }

    [Fact]
    public void NonParticipant_DoesNotSatisfy_ProjectOwnerOrCollaboratorRequirement_Throws_NotAllowed_ForPublicProject()
    {
        // Arrange:
        Project project = _dataDictionary.Projects.First(p => p.Title == "PublicProject");

        // Assert:
        Assert.Throws<NotAllowedException>(() => SUT.AuthorizeProjectOwnerOrCollaboratorRequirement(Guid.NewGuid(), project));
    }

    [Fact]
    public void NonParticipant_DoesNotSatisfy_ProjectOwnerOrCollaboratorRequirement_Throws_NotFound_ForPrivateProject()
    {
        // Arrange:
        Project project = _dataDictionary.Projects.First(p => p.Title == "PrivateProject");

        // Assert:
        Assert.Throws<EntityNotFoundException>(() => SUT.AuthorizeProjectOwnerOrCollaboratorRequirement(Guid.NewGuid(), project));
    }
}
