using ProjectManager.Application.Interfaces;
using ProjectManager.Domain.Entities;

namespace UnitTests.FakeRepositories;

public class FakeUserRepository : IUserRepository
{
    readonly List<User> _users;

    public FakeUserRepository(List<User> users)
    {
        _users = users;
    }

    internal void supplyUser(User user)
    {
        _users.Add(user);
    }

    public async Task<User?> GetById(Guid userId)
    {
        return _users.FirstOrDefault(u => u.Id == userId);
    }
}
