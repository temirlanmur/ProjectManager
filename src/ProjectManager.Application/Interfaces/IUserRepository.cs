using ProjectManager.Domain.Entities;

namespace ProjectManager.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid userId);
    Task<User?> GetByEmailAsync(string email);
    Task<User> SaveAsync(User user);
}
