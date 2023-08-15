using ProjectManager.Application.Interfaces;
using ProjectManager.Domain.Entities;
using UnitTests.Extensions;

namespace UnitTests.FakeRepositories;

public class FakeTaskCommentRepository : ITaskCommentRepository
{
    readonly DataDictionary _data;

    public FakeTaskCommentRepository(DataDictionary data)
    {
        _data = data;
    }

    public async Task DeleteAsync(TaskComment comment)
    {
        _data.TaskComments.Remove(comment);
    }

    public async Task<TaskComment> SaveAsync(TaskComment comment)
    {
        var existingComment = _data.TaskComments.FirstOrDefault(t => t.Id == comment.Id);

        if (existingComment is not null)
        {
            _data.TaskComments.Remove(existingComment);
        }

        _data.TaskComments.Add(comment.WithId(Guid.NewGuid()));

        return comment;
    }
}
