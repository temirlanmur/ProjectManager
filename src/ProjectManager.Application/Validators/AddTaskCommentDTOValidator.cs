using FluentValidation;
using ProjectManager.Application.DTOs.TaskDTOs;

namespace ProjectManager.Application.Validators;

public class AddTaskCommentDTOValidator : AbstractValidator<AddTaskCommentDTO>
{
    public AddTaskCommentDTOValidator()
    {
        RuleFor(d => d.ActorId).NotEmpty();
        RuleFor(d => d.ProjectId).NotEmpty();
        RuleFor(d => d.TaskId).NotEmpty();
        RuleFor(d => d.Text).NotEmpty().MaximumLength(2000);
    }
}
