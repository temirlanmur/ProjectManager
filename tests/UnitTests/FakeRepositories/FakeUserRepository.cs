using ProjectManager.Application.Interfaces;
using ProjectManager.Domain.Entities;
using System.Threading.Tasks;

namespace UnitTests.FakeRepositories;

public class FakeUserRepository : IUserRepository
{
    readonly DataDictionary _data;

    public FakeUserRepository(DataDictionary data)
    {
        _data = data;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return _data.Users.FirstOrDefault(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(Guid userId)
    {
        return _data.Users.FirstOrDefault(u => u.Id == userId);
    }

    public async Task<User> SaveAsync(User user)
    {
        var existingUser = _data.Users.FirstOrDefault(u => u.Id == user.Id);

        if (existingUser is not null)
        {
            _data.Users.Remove(existingUser);
        }

        _data.Users.Add(user);

        return user;
    }
}
