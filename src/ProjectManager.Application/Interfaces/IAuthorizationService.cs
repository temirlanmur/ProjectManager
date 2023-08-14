using ProjectManager.Domain.Entities;

namespace ProjectManager.Application.Interfaces;

public interface IAuthorizationService
{
    /// <summary>
    /// Checks if the given actor is project owner.
    /// If so, simply returns.
    /// Otherwise throws suitable exception.
    /// </summary>
    /// <param name="actorId"></param>
    /// <param name="project"></param>
    void ThrowIfNotProjectOwner(Guid actorId, Project project);

    /// <summary>
    /// Checks if the given actor is project owner or collaborator.
    /// If so, simply returns.
    /// Otherwise throws suitable exception.
    /// </summary>
    /// <param name="actorId"></param>
    /// <param name="project"></param>
    void ThrowIfNotProjectOwnerOrCollaborator(Guid actorId, Project project);
}
