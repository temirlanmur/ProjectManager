using ProjectManager.Domain.Exceptions;

namespace ProjectManager.Domain.Entities;

public class Project
{
    public Guid Id { get; init; }
    public Guid OwnerId { get; private set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsPublic { get; set; }

    public User Owner { get; private set; }

    private List<User> _collaborators = new();
    public IReadOnlyCollection<User> Collaborators => _collaborators.ToList();

    public Project(Guid ownerId, string title, string description = "", bool isPublic = false)
    {
        OwnerId = ownerId;
        Title = title;
        Description = description;
        IsPublic = isPublic;
    }

    /// <summary>
    /// Adds collaborator to the project.
    /// </summary>
    /// <param name="collaborator"></param>
    /// <exception cref="AlreadyCollaboratorException"></exception>
    public void AddCollaborator(User collaborator)
    {
        if (Collaborators.Any(c => c.Id == collaborator.Id))
        {
            throw new AlreadyCollaboratorException();
        }

        _collaborators.Add(collaborator);
    }
}
