using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Interfaces;
using ProjectManager.Domain.Entities;

namespace ProjectManager.Application.Services;

public class AuthorizationService : IAuthorizationService
{
    public void ThrowIfNotProjectOwnerOrCollaborator(Guid actorId, Project project)
    {
        if (project.OwnerId == actorId || project.Collaborators.Any(c => c.Id == actorId))
        {
            return;
        }

        if (project.IsPublic)
        {
            throw new NotAllowedException();
        }

        throw EntityNotFoundException.ForEntity(typeof(Project));
    }

    public void ThrowIfNotProjectOwner(Guid actorId, Project project)
    {
        if (project.OwnerId == actorId)
        {
            return;
        }

        if (project.IsPublic || project.Collaborators.Any(c => c.Id == actorId))
        {
            throw new NotAllowedException();
        }

        throw EntityNotFoundException.ForEntity(typeof(Project));
    }
}
