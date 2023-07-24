using ProjectManager.Application.Interfaces;
using ProjectManager.Domain.Entities;

namespace UnitTests.FakeRepositories;

public class FakeUserRepository : IUserRepository
{
    private List<User> _users;

    public FakeUserRepository(List<User> users)
    {
        _users = users;
    }

    public async Task<User?> GetById(Guid userId)
    {
        return _users.FirstOrDefault(u => u.Id == userId);
    }
}
