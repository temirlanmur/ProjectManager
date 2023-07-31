using ProjectManager.Domain.Exceptions;

namespace ProjectManager.Domain.Entities;

public class Project
{
    public Guid Id { get; private set; }
    public Guid OwnerId { get; private set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsPublic { get; set; }

    public User Owner { get; private set; }

    private List<User> _collaborators = new();
    public IReadOnlyCollection<User> Collaborators => _collaborators.ToList();

    private List<ProjectTask> _tasks = new();
    public IReadOnlyCollection<ProjectTask> Tasks => _tasks.ToList();

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

    /// <summary>
    /// Removes collaborator from the project.
    /// </summary>
    /// <param name="collaboratorId"></param>
    /// <exception cref="CollaboratorNotFoundException"></exception>
    public void RemoveCollaborator(Guid collaboratorId)
    {
        var collaborator = Collaborators.FirstOrDefault(c => c.Id == collaboratorId) ?? throw new CollaboratorNotFoundException();

        _collaborators.Remove(collaborator);
    }
}
