using FluentValidation;
using ProjectManager.Application.DTOs.TaskDTOs;

namespace ProjectManager.Application.Validators;

public class DeleteTaskCommentDTOValidator : AbstractValidator<DeleteTaskCommentDTO>
{
    public DeleteTaskCommentDTOValidator()
    {
        RuleFor(d => d.ActorId).NotEmpty();
        RuleFor(d => d.ProjectId).NotEmpty();
        RuleFor(d => d.TaskId).NotEmpty();
        RuleFor(d => d.TaskCommentId).NotEmpty();
    }
}
