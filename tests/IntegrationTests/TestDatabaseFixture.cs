using Microsoft.EntityFrameworkCore;
using ProjectManager.Domain.Entities;
using ProjectManager.Persistence.Data;

namespace IntegrationTests;

public class TestDatabaseFixture
{
    private const string TestDbConnectionString = @"Server=localhost;Database=ProjectManager;User Id=SA;Password=MyPassword!;TrustServerCertificate=true";

    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    public TestDatabaseFixture()
    {
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                using var context = CreateContext();

                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.AddRange(
                    new User("ProjectOwner", "Lastname", "email@email.com", "123123Abc"),
                    new User("ProjectCollaborator", "Lastname", "email@email.com", "123123Abc"),
                    new User("RandomUser", "Lastname", "email@email.com", "123123Abc"));
                context.SaveChanges();

                User owner = context.Users.First(u => u.FirstName == "ProjectOwner");
                User collaborator = context.Users.First(u => u.FirstName == "ProjectCollaborator");
                User randomUser = context.Users.First(u => u.FirstName == "RandomUser");

                context.Add(
                    new Project(owner.Id, "Project", isPublic: false));
                context.SaveChanges();

                Project project = context.Projects.First(p => p.Title == "Project");

                project.AddCollaborator(collaborator);                
                context.SaveChanges();

                context.Add(
                    new ProjectTask(project.Id, collaborator.Id, "Task"));
                context.SaveChanges();

                ProjectTask task = context.Tasks.First(t => t.Title == "Task");

                context.AddRange(
                    new TaskComment(task.Id, collaborator.Id, "Comment 1"),
                    new TaskComment(task.Id, collaborator.Id, "Comment 2"));
                context.SaveChanges();
            }
        }
    }

    public ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(TestDbConnectionString)
            .Options;

        return new ApplicationDbContext(options);
    }
}
