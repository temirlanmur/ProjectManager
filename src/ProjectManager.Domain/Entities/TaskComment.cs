﻿namespace ProjectManager.Domain.Entities;

public class TaskComment
{
    public Guid Id { get; private set; }
    public Guid AuthorId { get; private set; }
    public string Text { get; set; }

    public TaskComment(Guid authorId, string text)
    {
        AuthorId = authorId;
        Text = text;
    }
}
