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

    public Task Delete(Guid commentId)
    {
        throw new NotImplementedException();
    }

    public async Task<TaskComment> Save(TaskComment comment)
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
