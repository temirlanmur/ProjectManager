namespace ProjectManager.Domain.Entities;

public class Project
{
    public Guid Id { get; init; }
    public Guid OwnerId { get; private set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsPublic { get; set; }    

    public User Owner { get; private set; }
    public IReadOnlyCollection<User> Collaborators { get; private set; } = new List<User>();

    public Project(Guid ownerId, string title, string description = "", bool isPublic = false)
    {
        OwnerId = ownerId;
        Title = title;
        Description = description;
        IsPublic = isPublic;
    }
}
