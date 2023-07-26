using ProjectManager.Application.Interfaces;
using ProjectManager.Domain.Entities;

namespace UnitTests.FakeRepositories;

public class FakeUserRepository : IUserRepository
{
    readonly DataDictionary _data;

    public FakeUserRepository(DataDictionary data)
    {
        _data = data;
    }

    public async Task<User?> GetById(Guid userId)
    {
        return _data.Users.FirstOrDefault(u => u.Id == userId);
    }
}
